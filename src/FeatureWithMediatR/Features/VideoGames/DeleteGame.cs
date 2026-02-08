using Domain.VideoGame;
using DomainKernel;
using Infrastructure.Database;
using MediatR;

namespace FeatureWithMediatR.Features.VideoGames;

/// <summary>
/// 「ゲーム削除」機能の垂直スライス
/// </summary>
/// <remarks>
/// <para>
/// IDを指定してゲームを削除する。
/// 存在しないIDの場合は404 Not Foundを返却。
/// </para>
/// <para>
/// <strong>処理フロー:</strong><br/>
/// 1. Endpoint が HTTP DELETE リクエストを受信<br/>
/// 2. ID から Command を生成<br/>
/// 3. Handler がデータベースからエンティティを削除<br/>
/// 4. 成功時は 204 No Content、失敗時は 404 Not Found
/// </para>
/// </remarks>
public static class DeleteGame
{
    /// <summary>
    /// ゲーム削除コマンド
    /// </summary>
    /// <param name="Id">削除対象のゲームID</param>
    /// <remarks>
    /// 削除成功時はtrue、対象が存在しない場合はfalseを返す。
    /// </remarks>
    public record DeleteGameCommand(int Id) : IRequest<Result>;

    /// <summary>
    /// コマンドハンドラ（削除処理実行）
    /// </summary>
    public class Handler(ApplicationDbContext dbContext) : IRequestHandler<DeleteGameCommand, Result>
    {
        /// <summary>
        /// ゲーム削除処理を実行
        /// </summary>
        /// <param name="command">削除コマンド</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>削除成功時true、対象不在時false</returns>
        public async Task<Result> Handle(DeleteGameCommand command, CancellationToken cancellationToken)
        {
            var videoGame = await dbContext.VideoGames.FindAsync([command.Id], cancellationToken);

            if (videoGame is null)
            {
                return Result.Failure(VideoGameErrors.NotFound(command.Id));
            }

            dbContext.VideoGames.Remove(videoGame);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
