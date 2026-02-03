using Domain.Entities;
using Infrastructure.Database;
using Shared.Messaging;

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
    public async Task<CreateGameResponse> Handle(CreateGameCommand command, CancellationToken cancellationToken)
    {
        // Command → Entity への変換
        var videoGame = new VideoGame
        {
            Title = command.Title,
            Genre = command.Genre,
            ReleaseYear = command.ReleaseYear
        };

        // EF Core による永続化
        dbContext.VideoGames.Add(videoGame);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Entity → Response への変換
        return new CreateGameResponse(
            videoGame.Id,
            videoGame.Title,
            videoGame.Genre,
            videoGame.ReleaseYear
        );
    }
}
