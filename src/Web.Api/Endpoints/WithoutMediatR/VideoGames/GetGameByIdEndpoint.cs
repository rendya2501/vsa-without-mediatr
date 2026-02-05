using Carter;
using Web.Api.Endpoints;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

namespace Web.Api.Endpoints.WithoutMediatR.VideoGames;

public sealed class GetGameByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithoutMediatRGamesApi()
            .MapGet("/{id:int}", async (
                IQueryHandler<GetGameByIdQuery, GetGameByIdResponse> handler,
                int id,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetGameByIdQuery(id), cancellationToken);
                return result.ToOk();
            })
            .WithName(VideoGameRouteNames.WithoutMediatR.GetById)
            //.WithSummary("Get a video game by ID")
            .WithDescription("Retrieves a specific video game by its ID")
            .Produces<GetGameByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
