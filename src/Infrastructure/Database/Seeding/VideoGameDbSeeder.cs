using Domain.VideoGame;

namespace Infrastructure.Database.Seeding;

public sealed class VideoGameDbSeeder(VideoGameDbContext dbContext) : IDbSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // InMemoryデータベースの作成
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        // すでにデータが存在する場合はスキップ
        if (dbContext.VideoGames.Any())
        {
            return;
        }

        // シードデータの投入
        dbContext.VideoGames.AddRange(
            new VideoGame
            {
                Id = 1,
                Title = "The Legend of Zelda: Breath of the Wild",
                Genre = "Action",
                ReleaseYear = 2017
            },
            new VideoGame
            {
                Id = 2,
                Title = "The Witcher 3: Wild Hunt",
                Genre = "RPG",
                ReleaseYear = 2015
            },
            new VideoGame
            {
                Id = 3,
                Title = "DOOM Eternal",
                Genre = "Shooter",
                ReleaseYear = 2020
            },
            new VideoGame
            {
                Id = 4,
                Title = "Red Dead Redemption 2",
                Genre = "Adventure",
                ReleaseYear = 2018
            },
            new VideoGame
            {
                Id = 5,
                Title = "Civilization VI",
                Genre = "Strategy",
                ReleaseYear = 2016
            }
        );

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
