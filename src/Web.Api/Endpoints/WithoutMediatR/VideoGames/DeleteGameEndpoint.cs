using Carter;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

namespace Web.Api.Endpoints.WithoutMediatR.VideoGames;

public sealed class DeleteGameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithoutMediatRGamesApi()
            .MapDelete("/{id:int}", async (
                ICommandHandler<DeleteGameCommand> handler,
                int id,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeleteGameCommand(id), cancellationToken);
                return result.ToNoContent();
            })
            .WithName(VideoGameRouteNames.WithoutMediatR.Delete)
            //.WithSummary("Delete a video game")
            .WithDescription("Deletes a video game by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
