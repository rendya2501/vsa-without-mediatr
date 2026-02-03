using Carter;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

public sealed class GetGameByIdEndpoint2 : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapGet("/{id:int}", async (
                int id,
                IQueryHandler<GetGameByIdQuery, GetGameByIdResponse?> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(new GetGameByIdQuery(id), ct);

                if (result is null)
                {
                    return Results.NotFound($"Video game with id {id} not found.");
                }

                return Results.Ok(result);
            })
            .WithName(VideoGameConstants.RouteNames.GetById)
            //.WithSummary("Get a video game by ID")
            .WithDescription("Retrieves a specific video game by its ID")
            .Produces<GetGameByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
