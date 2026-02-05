using Carter;
using FeatureShared.Extensions;
using FeatureWithMediatR.Features.WeatherForecast.GetWeatherForecast;
using MediatR;

namespace Web.Api.Endpoints.WithMediatR.WeatherForecasts;

/// <summary>
/// WeatherForecast機能のエンドポイント定義モジュール
/// </summary>
public sealed class GetWeatherForecastEndpoint : ICarterModule
{
    /// <summary>
    /// エンドポイントルートを登録
    /// </summary>
    /// <param name="app">エンドポイントルートビルダー</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET 5日間の天気予報を取得
        app.MapWithMediatRWeatherApi()
            .MapGet("/", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new WeatherForecastQuery(), cancellationToken);
                return result.ToOk();
            })
            .WithName("GetWeatherForecast_WithMediatR")
            .WithDescription("Retrieves a 5-day weather forecast with temperature and conditions")
            .Produces<IEnumerable<WeatherForecastResponse>>(StatusCodes.Status200OK);


        //// GET 5日間の天気予報を取得
        //app.MapGet("/api/weather-forecast-mediatr/", async
        //    (ISender sender,
        //    CancellationToken cancellationToken) =>
        //{
        //    var result = await sender.Send(new WeatherForecastQuery(), cancellationToken);
        //    return result.ToOk();
        //})
        //.WithTags("WithMediatR")
        ////.WithName("GetWeatherForecast_MediatR")
        ////.WithDescription("Retrieves a 5-day weather forecast with temperature and conditions")
        //.Produces<IEnumerable<WeatherForecastResponse>>(StatusCodes.Status200OK);
    }
}
