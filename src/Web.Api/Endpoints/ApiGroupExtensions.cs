namespace Web.Api.Endpoints;

public static class ApiGroupExtensions
{
    public static RouteGroupBuilder MapWithMediatRWeatherApi(this IEndpointRouteBuilder app)
        => app.MapGroup("/api/with-mediatr/weather-forecast").WithTags("WithMediatR");
    public static RouteGroupBuilder MapWithMediatRGamesApi(this IEndpointRouteBuilder app)
        => app.MapGroup("/api/with-mediatr/games").WithTags("WithMediatR");


    public static RouteGroupBuilder MapWithoutMediatRWeatherApi(this IEndpointRouteBuilder app)
        => app.MapGroup("/api/without-mediatr/weather-forecast").WithTags("WithoutMediatR");
    public static RouteGroupBuilder MapWithoutMediatRGamesApi(this IEndpointRouteBuilder app)
        => app.MapGroup("/api/without-mediatr/games").WithTags("WithoutMediatR");
}
