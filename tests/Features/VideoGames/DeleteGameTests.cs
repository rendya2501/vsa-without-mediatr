using Application.Features.VideoGames;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VideoGameApiVsa.Entities;
using Web.Api.Data;

namespace VideoGameApiVsa.Tests.Features.VideoGames;

/// <summary>
/// DeleteGame機能のテストクラス
/// </summary>
public class DeleteGameTests
{
    /// <summary>
    /// ゲームが存在する場合、ゲームを削除しtrueを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldDeleteGame_WhenGameExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var game = new VideoGame { Id = 1, Title = "Game to Delete", Genre = "Action", ReleaseYear = 2020 };
        dbContext.VideoGames.Add(game);
        await dbContext.SaveChangesAsync();

        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var gameInDb = await dbContext.VideoGames.FindAsync([1]);
        gameInDb.Should().BeNull();
    }

    /// <summary>
    /// ゲームが存在しない場合、falseを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenGameDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// 複数のゲームが存在する場合、指定されたIDのゲームのみを削除することを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldDeleteOnlySpecifiedGame_WhenMultipleGamesExist()
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

        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var deletedGame = await dbContext.VideoGames.FindAsync([2]);
        deletedGame.Should().BeNull();

        var remainingGames = await dbContext.VideoGames.ToListAsync();
        remainingGames.Should().HaveCount(2);
        remainingGames.Should().Contain(g => g.Id == 1);
        remainingGames.Should().Contain(g => g.Id == 3);
        remainingGames.Should().NotContain(g => g.Id == 2);
    }

    /// <summary>
    /// 同じゲームを二回削除しようとした場合、二回目はfalseを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDeletingSameGameTwice()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var game = new VideoGame { Id = 1, Title = "Game to Delete", Genre = "Action", ReleaseYear = 2020 };
        dbContext.VideoGames.Add(game);
        await dbContext.SaveChangesAsync();

        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(1);

        // Act: 一回目の削除
        var firstResult = await handler.Handle(command, CancellationToken.None);

        // Act: 二回目の削除
        var secondResult = await handler.Handle(command, CancellationToken.None);

        // Assert
        firstResult.Should().BeTrue();
        secondResult.Should().BeFalse();
    }

    /// <summary>
    /// ID=0の場合、falseを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenIdIsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// 負のIDの場合、falseを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenIdIsNegative()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(-1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// 削除後のデータベース状態が正しいことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldMaintainDatabaseIntegrity_AfterDeletion()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        dbContext.VideoGames.AddRange(
            new VideoGame { Id = 1, Title = "Game 1", Genre = "Action", ReleaseYear = 2020 },
            new VideoGame { Id = 2, Title = "Game 2", Genre = "RPG", ReleaseYear = 2021 }
        );
        await dbContext.SaveChangesAsync();

        var initialCount = await dbContext.VideoGames.CountAsync();
        initialCount.Should().Be(2);

        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var finalCount = await dbContext.VideoGames.CountAsync();
        finalCount.Should().Be(1);

        var remainingGame = await dbContext.VideoGames.SingleAsync();
        remainingGame.Id.Should().Be(2);
        remainingGame.Title.Should().Be("Game 2");
    }

    /// <summary>
    /// 非常に大きなIDの場合、falseを返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenIdIsVeryLarge()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new VideoGameDbContext(options);
        var handler = new DeleteGame.Handler(dbContext);
        var command = new DeleteGame.DeleteGameCommand(int.MaxValue);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}