using Carter;
using FeatureShared.Extensions;
using FeatureShared.Infrastructure;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName(VideoGameRounteNames.GetAll)
            //.WithSummary("Get all video games")
            .WithDescription("Retrieves a list of all video games in the database")
            .Produces<IEnumerable<GetAllGamesResponse>>(StatusCodes.Status200OK);
    }
}
