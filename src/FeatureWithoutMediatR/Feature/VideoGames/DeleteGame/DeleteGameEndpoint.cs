using Carter;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

public sealed class DeleteGameEndpoint2 : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapDelete("/{id:int}", async (
                int id,
                ICommandHandler<DeleteGameCommand, bool> handler,
                CancellationToken ct) =>
            {
                var deleted = await handler.Handle(new DeleteGameCommand(id), ct);

                if (deleted is false)
                {
                    return Results.NotFound($"Video game with id {id} not found.");
                }

                return Results.NoContent();
            })
            .WithName(VideoGameConstants.RouteNames.Delete)
            //.WithSummary("Delete a video game")
            .WithDescription("Deletes a video game by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
