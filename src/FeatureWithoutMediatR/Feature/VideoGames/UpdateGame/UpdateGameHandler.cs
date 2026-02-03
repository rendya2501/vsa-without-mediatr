using Infrastructure.Database;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;

/// <summary>
/// コマンドハンドラ（更新処理実行）
/// </summary>
internal sealed class UpdateGameHandler(VideoGameDbContext dbContext)
    : ICommandHandler<UpdateGameCommand, UpdateGameResponse?>
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
    public async Task<UpdateGameResponse?> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
    {
        var videoGame = await dbContext.VideoGames.FindAsync([command.Id], cancellationToken);

        if (videoGame is null)
        {
            return null;
        }

        videoGame.Title = command.Title;
        videoGame.Genre = command.Genre;
        videoGame.ReleaseYear = command.ReleaseYear;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateGameResponse(videoGame.Id, videoGame.Title, videoGame.Genre, videoGame.ReleaseYear);
    }
}
