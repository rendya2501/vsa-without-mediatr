using FeatureWithMediatR.Features.VideoGames;
using FeatureWithoutMediatR.Feature.VideoGames.CreateGame;
using FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;
using FeatureWithoutMediatR.Feature.VideoGames.GetGameById;
using FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Features.VideoGames;

/// <summary>
/// VideoGamesの統合テストクラス
/// </summary>
public class VideoGamesIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    /// <summary>
    /// 有効なリクエストの場合、ゲームを作成し201を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task CreateGame_ShouldReturn201_WhenValidRequest()
    {
        // Arrange
        var request = new CreateGame.CreateGameRequest("Integration Test Game", "Action", 2023);

        // Act
        var response = await _client.PostAsJsonAsync("/api/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdGame = await response.Content.ReadFromJsonAsync<CreateGame.CreateGameResponse>();
        createdGame.Should().NotBeNull();
        createdGame!.Title.Should().Be("Integration Test Game");
        createdGame.Genre.Should().Be("Action");
        createdGame.ReleaseYear.Should().Be(2023);
    }

    /// <summary>
    /// タイトルが空の無効なリクエストの場合、400を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task CreateGame_ShouldReturn400_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateGame.CreateGameRequest("", "Action", 2023);

        // Act
        var response = await _client.PostAsJsonAsync("/api/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// リリース年が未来の無効なリクエストの場合、400を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task CreateGame_ShouldReturn400_WhenReleaseYearIsInFuture()
    {
        // Arrange
        var request = new CreateGame.CreateGameRequest("Test Game", "Action", DateTime.Now.Year + 1);

        // Act
        var response = await _client.PostAsJsonAsync("/api/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// ゲーム一覧を取得し200を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task GetAllGames_ShouldReturn200_WithGames()
    {
        // Act
        var response = await _client.GetAsync("/api/games");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GetAllGames.GetAllGamesResponse>>();
        games.Should().NotBeNull();
        games.Should().HaveCountGreaterThan(0); // シードデータが存在する想定
    }

    /// <summary>
    /// 存在しないゲームを取得しようとした場合、404を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task GetGameById_ShouldReturn404_WhenGameNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/games/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// 作成したゲームを取得できることを確認するテスト（エンドツーエンド）
    /// </summary>
    [Fact]
    public async Task CreateAndGetGame_ShouldReturnCreatedGame()
    {
        // Arrange & Act: ゲーム作成
        var createRequest = new CreateGame.CreateGameRequest("E2E Test Game", "Adventure", 2022);
        var createResponse = await _client.PostAsJsonAsync("/api/games", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdGame = await createResponse.Content.ReadFromJsonAsync<CreateGame.CreateGameResponse>();
        createdGame.Should().NotBeNull();

        // Act: 作成したゲームを取得
        var getResponse = await _client.GetAsync($"/api/games/{createdGame!.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedGame = await getResponse.Content.ReadFromJsonAsync<GetGameById.GetGameByIdResponse>();
        retrievedGame.Should().NotBeNull();
        retrievedGame!.Id.Should().Be(createdGame.Id);
        retrievedGame.Title.Should().Be("E2E Test Game");
        retrievedGame.Genre.Should().Be("Adventure");
        retrievedGame.ReleaseYear.Should().Be(2022);
    }

    /// <summary>
    /// 作成、更新、取得、削除の一連の操作が正しく動作することを確認するテスト（エンドツーエンド）
    /// </summary>
    [Fact]
    public async Task FullCrud_ShouldWorkCorrectly()
    {
        // 1. Create
        var createRequest = new CreateGame.CreateGameRequest("CRUD Test Game", "Strategy", 2021);
        var createResponse = await _client.PostAsJsonAsync("/api/games", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdGame = await createResponse.Content.ReadFromJsonAsync<CreateGame.CreateGameResponse>();
        var gameId = createdGame!.Id;

        // 2. Update
        var updateRequest = new UpdateGame.UpdateGameRequest("Updated CRUD Game", "RPG", 2022);
        var updateResponse = await _client.PutAsJsonAsync($"/api/games/{gameId}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedGame = await updateResponse.Content.ReadFromJsonAsync<UpdateGame.UpdateGameResponse>();
        updatedGame.Should().NotBeNull();
        updatedGame!.Title.Should().Be("Updated CRUD Game");
        updatedGame.Genre.Should().Be("RPG");
        updatedGame.ReleaseYear.Should().Be(2022);

        // 3. Get (更新後の確認)
        var getResponse = await _client.GetAsync($"/api/games/{gameId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedGame = await getResponse.Content.ReadFromJsonAsync<GetGameById.GetGameByIdResponse>();
        retrievedGame.Should().NotBeNull();
        retrievedGame!.Title.Should().Be("Updated CRUD Game");
        retrievedGame.Genre.Should().Be("RPG");

        // 4. Delete
        var deleteResponse = await _client.DeleteAsync($"/api/games/{gameId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5. Get (削除後の確認)
        var getAfterDeleteResponse = await _client.GetAsync($"/api/games/{gameId}");
        getAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// 存在しないゲームを更新しようとした場合、404を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task UpdateGame_ShouldReturn404_WhenGameNotFound()
    {
        // Arrange
        var updateRequest = new UpdateGame.UpdateGameRequest("Non-existent Game", "Action", 2023);

        // Act
        var response = await _client.PutAsJsonAsync("/api/games/999999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// 無効なデータで更新しようとした場合、400を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task UpdateGame_ShouldReturn400_WhenInvalidData()
    {
        // Arrange: まずゲームを作成
        var createRequest = new CreateGame.CreateGameRequest("Game to Update", "Action", 2020);
        var createResponse = await _client.PostAsJsonAsync("/api/games", createRequest);
        var createdGame = await createResponse.Content.ReadFromJsonAsync<CreateGame.CreateGameResponse>();

        // Act: 無効なデータで更新
        var updateRequest = new UpdateGame.UpdateGameRequest("", "Action", 2023); // タイトルが空
        var updateResponse = await _client.PutAsJsonAsync($"/api/games/{createdGame!.Id}", updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// 存在しないゲームを削除しようとした場合、404を返すことを確認するテスト
    /// </summary>
    [Fact]
    public async Task DeleteGame_ShouldReturn404_WhenGameNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/games/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// 同時に複数のゲームを作成できることを確認するテスト
    /// </summary>
    [Fact]
    public async Task CreateMultipleGames_ShouldSucceed()
    {
        // Arrange
        var games = new[]
        {
            new CreateGame.CreateGameRequest("Game A", "Action", 2020),
            new CreateGame.CreateGameRequest("Game B", "RPG", 2021),
            new CreateGame.CreateGameRequest("Game C", "Strategy", 2022)
        };

        // Act
        var tasks = games.Select(g => _client.PostAsJsonAsync("/api/games", g)).ToArray();
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.Created));
    }
}