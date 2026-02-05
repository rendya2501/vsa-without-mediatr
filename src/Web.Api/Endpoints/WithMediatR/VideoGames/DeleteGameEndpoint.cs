using Carter;
using Web.Api.Endpoints;
using FeatureShared.Extensions;
using MediatR;
using static FeatureWithMediatR.Features.VideoGames.DeleteGame;

namespace Web.Api.Endpoints.WithMediatR.VideoGames;

public sealed class DeleteGameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithMediatRGamesApi()
            .MapDelete("/{id:int}", async (
                ISender sender, 
                int id, 
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteGameCommand(id), cancellationToken);
                return result.ToNoContent();
            })
            .WithName(VideoGameRouteNames.WithMediatR.Delete)
            //.WithSummary("Delete a video game")
            .WithDescription("Deletes a video game by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
