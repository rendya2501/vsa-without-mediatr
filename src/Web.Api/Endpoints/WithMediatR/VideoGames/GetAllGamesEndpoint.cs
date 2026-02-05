using Carter;
using Web.Api.Endpoints;
using FeatureShared.Extensions;
using MediatR;
using static FeatureWithMediatR.Features.VideoGames.GetAllGames;

namespace Web.Api.Endpoints.WithMediatR.VideoGames;

public sealed class GetAllGamesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithMediatRGamesApi()
            .MapGet("/", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllGamesQuery(), cancellationToken);
                return result.ToOk();
            })
            .WithName(VideoGameRouteNames.WithMediatR.GetAll)
            //.WithSummary("Get all video games")
            .WithDescription("Retrieves a list of all video games in the database")
            .Produces<IEnumerable<GetAllGamesResponse>>(StatusCodes.Status200OK);

    }
}
