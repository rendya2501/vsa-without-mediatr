using Carter;
using FeatureShared.Extensions;
using FeatureShared.Infrastructure;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

public sealed class GetGameByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapGet("/{id:int}", async (
                int id,
                IQueryHandler<GetGameByIdQuery, GetGameByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetGameByIdQuery(id), cancellationToken);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName(VideoGameRounteNames.GetById)
            //.WithSummary("Get a video game by ID")
            .WithDescription("Retrieves a specific video game by its ID")
            .Produces<GetGameByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
