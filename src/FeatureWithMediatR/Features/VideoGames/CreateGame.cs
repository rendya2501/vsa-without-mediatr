using Domain.VideoGame;
using DomainKernel;
using FluentValidation;
using Infrastructure.Database;
using MediatR;

namespace FeatureWithMediatR.Features.VideoGames;

/// <summary>
/// 「ゲーム作成」機能の垂直スライス
/// </summary>
/// <remarks>
/// <para>
/// Request / Command / Validator / Handler / Endpoint を1ファイルに集約。
/// このファイルだけでゲーム作成機能の全体像を把握できる。
/// </para>
/// <para>
/// <strong>処理フロー:</strong><br/>
/// 1. Endpoint が HTTP リクエストを受信<br/>
/// 2. Request → Command へ変換<br/>
/// 3. ValidationBehavior が Validator を実行<br/>
/// 4. Handler がビジネスロジックを実行<br/>
/// 5. Response を HTTP 201 Created で返却
/// </para>
/// </remarks>
public static class CreateGame
{
    /// <summary>
    /// ゲーム作成コマンド（内部処理用）
    /// </summary>
    /// <param name="Title">ゲームタイトル</param>
    /// <param name="Genre">ゲームジャンル</param>
    /// <param name="ReleaseYear">リリース年</param>
    /// <remarks>
    /// MediatR経由で処理されるアプリケーション内部のメッセージ。
    /// ValidationBehaviorにより自動的にValidatorが適用される。
    /// </remarks>
    public record CreateGameCommand(
        string Title,
        string Genre,
        int ReleaseYear) : IRequest<Result<CreateGameResponse>>;

    /// <summary>
    /// ゲーム作成レスポンス
    /// </summary>
    /// <param name="Id">作成されたゲームのID</param>
    /// <param name="Title">ゲームタイトル</param>
    /// <param name="Genre">ゲームジャンル</param>
    /// <param name="ReleaseYear">リリース年</param>
    /// <remarks>
    /// Entityを直接公開せず、API専用のDTOとして定義。
    /// 将来的なEntity変更がAPIに影響しないよう分離している。
    /// </remarks>
    public record CreateGameResponse(
        int Id,
        string Title,
        string Genre,
        int ReleaseYear);

    /// <summary>
    /// コマンド検証ルール
    /// </summary>
    /// <remarks>
    /// MediatR Pipelineで自動実行される。
    /// 検証失敗時はValidationExceptionをスローし、
    /// グローバル例外ハンドラでProblemDetails形式に変換される。
    /// </remarks>
    public class Validator : AbstractValidator<CreateGameCommand>
    {
        public Validator()
        {
            // タイトルは必須 & 最大文字数
            RuleFor(x => x.Title)
                .NotEmpty()// .WithMessage("Title is required.")
                .MaximumLength(VideoGameValidationRules.Title.MaxLength);// .WithMessage("Length is Max100.");

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
    /// コマンドハンドラ（ビジネスロジック実行）
    /// </summary>
    public class Handler(ApplicationDbContext dbContext)
        : IRequestHandler<CreateGameCommand, Result<CreateGameResponse>>
    {
        /// <summary>
        /// ゲーム作成処理を実行
        /// </summary>
        /// <param name="command">作成コマンド</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>作成されたゲーム情報</returns>
        public async Task<Result<CreateGameResponse>> Handle(
            CreateGameCommand command,
            CancellationToken cancellationToken)
        {
            var videoGame = new VideoGame
            {
                Title = command.Title,
                Genre = command.Genre,
                ReleaseYear = command.ReleaseYear
            };

            dbContext.VideoGames.Add(videoGame);
            await dbContext.SaveChangesAsync(cancellationToken);

            var response = new CreateGameResponse(
                videoGame.Id,
                videoGame.Title,
                videoGame.Genre,
                videoGame.ReleaseYear
            );

            return Result.Success(response);
        }
    }
}
