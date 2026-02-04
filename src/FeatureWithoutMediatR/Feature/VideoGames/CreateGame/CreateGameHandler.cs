using Domain.VideoGame;
using DomainKernel;
using FeatureShared.Messaging;
using Infrastructure.Database;

namespace FeatureWithoutMediatR.Feature.VideoGames.CreateGame;

/// <summary>
/// コマンドハンドラ（ビジネスロジック実行）
/// </summary>
internal sealed class Handler(VideoGameDbContext dbContext)
    : ICommandHandler<CreateGameCommand, CreateGameResponse>
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
            videoGame.ReleaseYear);

        return Result.Success(response);
    }
}
