using Domain.VideoGame;
using DomainKernel;
using FeatureShared.Messaging;
using Infrastructure.Database;

namespace FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;

/// <summary>
/// コマンドハンドラ（更新処理実行）
/// </summary>
internal sealed class UpdateGameHandler(ApplicationDbContext dbContext)
    : ICommandHandler<UpdateGameCommand, UpdateGameResponse>
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
            return Result.Failure<UpdateGameResponse>(VideoGameErrors.NotFound(command.Id));
        }

        videoGame.Title = command.Title;
        videoGame.Genre = command.Genre;
        videoGame.ReleaseYear = command.ReleaseYear;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new UpdateGameResponse(
            videoGame.Id,
            videoGame.Title,
            videoGame.Genre,
            videoGame.ReleaseYear);

        return Result.Success(response);
    }
}
