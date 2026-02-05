using Carter;
using Domain.VideoGame;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;

namespace Web.Api.Endpoints.WithoutMediatR.VideoGames;

public sealed class UpdateGameEndpoint : ICarterModule
{
    /// <summary>
    /// ゲーム更新リクエスト（外部APIインターフェース）
    /// </summary>
    /// <param name="Title">新しいゲームタイトル（最大100文字）</param>
    /// <param name="Genre">新しいゲームジャンル（最大50文字）</param>
    /// <param name="ReleaseYear">新しいリリース年（1950年以降）</param>
    /// <remarks>
    /// Idはルートパラメータから取得するため、ボディには含めない。
    /// CreateGameRequestと構造を合わせることで、API仕様の一貫性を保つ。
    /// </remarks>
    public record UpdateGameRequest(
        string Title,
        string Genre,
        int ReleaseYear = VideoGameValidationRules.ReleaseYear.DefaultValue);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithoutMediatRGamesApi()
            .MapPut("/{id:int}", async (
                ICommandHandler<UpdateGameCommand, UpdateGameResponse> handler,
                int id,
                UpdateGameRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateGameCommand(id, request.Title, request.Genre, request.ReleaseYear);

                var result = await handler.Handle(command, cancellationToken);

                return result.ToOk();
            })
            .WithName(VideoGameRounteNames.Update)
            //.WithSummary("Update an existing video game")
            .WithDescription("Updates an existing video game by its ID")
            .Produces<UpdateGameResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
