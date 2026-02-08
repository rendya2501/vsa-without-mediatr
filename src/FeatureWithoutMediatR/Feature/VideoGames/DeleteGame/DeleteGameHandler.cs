using Domain.VideoGame;
using DomainKernel;
using FeatureShared.Messaging;
using Infrastructure.Database;

namespace FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

/// <summary>
/// コマンドハンドラ（削除処理実行）
/// </summary>
internal sealed class DeleteGameHandler(ApplicationDbContext dbContext)
    : ICommandHandler<DeleteGameCommand>
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

        // domain events could be dispatched here if needed
        // todoItem.Raise(new TodoItemDeletedDomainEvent(todoItem.Id));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
