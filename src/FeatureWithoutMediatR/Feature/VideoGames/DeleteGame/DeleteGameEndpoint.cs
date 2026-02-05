using Carter;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

public sealed class DeleteGameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapDelete("/{id:int}", async (
                int id,
                ICommandHandler<DeleteGameCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeleteGameCommand(id), cancellationToken);
                return result.ToNoContent();
            })
            .WithName(VideoGameRounteNames.Delete)
            //.WithSummary("Delete a video game")
            .WithDescription("Deletes a video game by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
