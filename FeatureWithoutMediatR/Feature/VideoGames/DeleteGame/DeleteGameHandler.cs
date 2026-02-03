using Infrastructure.Database;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

/// <summary>
/// コマンドハンドラ（削除処理実行）
/// </summary>
internal sealed class DeleteGameHandler(VideoGameDbContext dbContext)
    : ICommandHandler<DeleteGameCommand, bool>
{
    /// <summary>
    /// ゲーム削除処理を実行
    /// </summary>
    /// <param name="command">削除コマンド</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>削除成功時true、対象不在時false</returns>
    public async Task<bool> Handle(DeleteGameCommand command, CancellationToken ct)
    {
        var videoGame = await dbContext.VideoGames.FindAsync([command.Id], ct);

        if (videoGame is null)
        {
            return false;
        }

        dbContext.VideoGames.Remove(videoGame);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}
