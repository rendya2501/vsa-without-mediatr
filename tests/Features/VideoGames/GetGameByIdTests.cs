using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VideoGameApiVsa.Entities;
using Web.Api.Data;
using Web.Api.Features.VideoGames;

namespace VideoGameApiVsa.Tests.Features.VideoGames;

/// <summary>
/// GetGameById機能のテストクラス
/// </summary>
public class GetGameByIdTests
{
    /// <summary>
    /// ゲームが存在する場合、指定されたIDのゲームを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var game = new VideoGame { Id = 1, Title = "Test Game", Genre = "Action", ReleaseYear = 2020 };
        dbContext.VideoGames.Add(game);
        await dbContext.SaveChangesAsync();

        var handler = new GetGameById.Handler(dbContext);
        var query = new GetGameById.GetGameByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Game");
        result.Genre.Should().Be("Action");
        result.ReleaseYear.Should().Be(2020);
    }

    /// <summary>
    /// ゲームが存在しない場合、nullを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new GetGameById.Handler(dbContext);
        var query = new GetGameById.GetGameByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// 複数のゲームが存在する場合、指定されたIDのゲームのみを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnCorrectGame_WhenMultipleGamesExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        dbContext.VideoGames.AddRange(
            new VideoGame { Id = 1, Title = "Game 1", Genre = "Action", ReleaseYear = 2020 },
            new VideoGame { Id = 2, Title = "Game 2", Genre = "RPG", ReleaseYear = 2021 },
            new VideoGame { Id = 3, Title = "Game 3", Genre = "Strategy", ReleaseYear = 2022 }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetGameById.Handler(dbContext);
        var query = new GetGameById.GetGameByIdQuery(2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.Title.Should().Be("Game 2");
        result.Genre.Should().Be("RPG");
        result.ReleaseYear.Should().Be(2021);
    }

    /// <summary>
    /// ID=0の場合、nullを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new GetGameById.Handler(dbContext);
        var query = new GetGameById.GetGameByIdQuery(0);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// 負のIDの場合、nullを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsNegative()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new GetGameById.Handler(dbContext);
        var query = new GetGameById.GetGameByIdQuery(-1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
