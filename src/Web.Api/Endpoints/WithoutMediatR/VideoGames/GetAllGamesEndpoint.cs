using Carter;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;

namespace Web.Api.Endpoints.WithoutMediatR.VideoGames;

public sealed class GetAllGamesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithoutMediatRGamesApi()
            .MapGet("/", async (
                IQueryHandler<GetAllGamesQuery, IEnumerable<GetAllGamesResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetAllGamesQuery(), cancellationToken);
                return result.ToOk();
            })
            .WithName(VideoGameRouteNames.GetAll)
            //.WithSummary("Get all video games")
            .WithDescription("Retrieves a list of all video games in the database")
            .Produces<IEnumerable<GetAllGamesResponse>>(StatusCodes.Status200OK);
    }
}
