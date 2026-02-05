using Carter;
using FeatureShared.Extensions;
using MediatR;
using static FeatureWithMediatR.Features.VideoGames.GetGameById;

namespace Web.Api.Endpoints.WithMediatR.VideoGames;

public sealed class GetGameByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithMediatRGamesApi()
            .MapGet("/{id:int}", async (
                ISender sender, 
                int id, 
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetGameByIdQuery(id), cancellationToken);
                return result.ToOk();
            })
            .WithName(VideoGameRounteNames.GetById)
            //.WithSummary("Get a video game by ID")
            .WithDescription("Retrieves a specific video game by its ID")
            .Produces<GetGameByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
