using Carter;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;

public sealed class GetAllGamesEndpoint2 : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapGet("/", async (
                IQueryHandler<GetAllGamesQuery, IEnumerable<GetAllGamesResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetAllGamesQuery(), cancellationToken);
                return Results.Ok(result);
            })
            .WithName(VideoGameConstants.RouteNames.GetAll)
            //.WithSummary("Get all video games")
            .WithDescription("Retrieves a list of all video games in the database")
            .Produces<IEnumerable<GetAllGamesResponse>>(StatusCodes.Status200OK);
    }
}
