using Microsoft.AspNetCore.Routing;

namespace FeatureWithoutMediatR.Extension;

public static class ApiGroupExtensions
{
    public static RouteGroupBuilder MapWithoutMediatRGamesApi(this IEndpointRouteBuilder app)
    {
        return app.MapApiGroup("/api/games2", "WithoutMediatR");
    }
}
