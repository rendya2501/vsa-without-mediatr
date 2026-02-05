using Domain.VideoGame;
using DomainKernel;
using FluentValidation;
using Infrastructure.Database;
using MediatR;

namespace FeatureWithMediatR.Features.VideoGames;

/// <summary>
/// 「ゲーム更新」機能の垂直スライス
/// </summary>
/// <remarks>
/// <para>
/// IDを指定して既存ゲームの情報を更新する。
/// 存在しないIDの場合は404 Not Foundを返却。
/// </para>
/// <para>
/// <strong>処理フロー:</strong><br/>
/// 1. Endpoint が HTTP PUT リクエストを受信<br/>
/// 2. ルートパラメータとボディからCommandを生成<br/>
/// 3. ValidationBehavior が Validator を実行<br/>
/// 4. Handler が既存エンティティを検索・更新<br/>
/// 5. 成功時は 200 OK + 更新後情報、不在時は 404 Not Found
/// </para>
/// <para>
/// <strong>設計ポリシー:</strong><br/>
/// - 部分更新(PATCH)ではなく完全更新(PUT)を採用<br/>
/// - すべてのフィールドを必須とし、バリデーションルールを統一<br/>
/// - Idは不変のため更新対象外
/// </para>
/// </remarks>
public static class UpdateGame
{
    /// <summary>
    /// ゲーム更新コマンド（内部処理用）
    /// </summary>
    /// <param name="Id">更新対象のゲームID</param>
    /// <param name="Title">新しいゲームタイトル</param>
    /// <param name="Genre">新しいゲームジャンル</param>
    /// <param name="ReleaseYear">新しいリリース年</param>
    /// <remarks>
    /// ルートパラメータとリクエストボディを結合したコマンド。
    /// ValidationBehaviorにより自動的にValidatorが適用される。
    /// </remarks>
    public record UpdateGameCommand(
        int Id,
        string Title,
        string Genre,
        int ReleaseYear) : IRequest<Result<UpdateGameResponse>>;

    /// <summary>
    /// ゲーム更新レスポンス
    /// </summary>
    /// <param name="Id">更新されたゲームのID</param>
    /// <param name="Title">更新後のゲームタイトル</param>
    /// <param name="Genre">更新後のゲームジャンル</param>
    /// <param name="ReleaseYear">更新後のリリース年</param>
    /// <remarks>
    /// 更新後の完全な情報を返却することで、クライアント側での再取得を不要にする。
    /// </remarks>
    public record UpdateGameResponse(
        int Id,
        string Title,
        string Genre,
        int ReleaseYear);

    /// <summary>
    /// コマンド検証ルール
    /// </summary>
    /// <remarks>
    /// CreateGameと同一のバリデーションルールを適用。
    /// 作成と更新で整合性を保つことで、クライアント側の実装を簡素化。
    /// </remarks>
    public class Validator : AbstractValidator<UpdateGameCommand>
    {
        public Validator()
        {
            // タイトルは必須 & 最大文字数
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(VideoGameValidationRules.Title.MaxLength);

            // ジャンルは必須 & 最大文字数
            RuleFor(x => x.Genre)
                .NotEmpty()
                .MaximumLength(VideoGameValidationRules.Genre.MaxLength);

            // リリース年は現実的な範囲に制限
            RuleFor(x => x.ReleaseYear)
                .InclusiveBetween(VideoGameValidationRules.ReleaseYear.MinValue, DateTime.Now.Year);
        }
    }

    /// <summary>
    /// コマンドハンドラ（更新処理実行）
    /// </summary>
    public class Handler(VideoGameDbContext dbContext)
        : IRequestHandler<UpdateGameCommand, Result<UpdateGameResponse>>
    {
        /// <summary>
        /// ゲーム更新処理を実行
        /// </summary>
        /// <param name="command">更新コマンド</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>更新後のゲーム情報、または対象不在時はnull</returns>
        /// <remarks>
        /// EF Coreの変更追跡により、プロパティ変更後のSaveChangesAsyncで
        /// 自動的にUPDATE文が発行される。
        /// </remarks>
        public async Task<Result<UpdateGameResponse>> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var videoGame = await dbContext.VideoGames.FindAsync([command.Id], cancellationToken);

            if (videoGame is null)
            {
                return VideoGameErrors.NotFound(command.Id);
            }

            videoGame.Title = command.Title;
            videoGame.Genre = command.Genre;
            videoGame.ReleaseYear = command.ReleaseYear;

            await dbContext.SaveChangesAsync(cancellationToken);

            var result = new UpdateGameResponse(videoGame.Id, videoGame.Title, videoGame.Genre, videoGame.ReleaseYear);

            return Result.Success(result);
        }
    }
}
