using Carter;
using FeatureShared.Extensions;
using FeatureShared.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithoutMediatR.Feature.WeatherForecast.GetWeatherForecast;

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
        app.MapGet("api/weather-forecast-self-maid/", async (
            IQueryHandler<WeatherForecastQuery, IEnumerable<WeatherForecastResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new WeatherForecastQuery(), cancellationToken);
            return result.ToOk();
        })
        .WithTags("WithoutMediatR")
        //.WithName("GetWeatherForecast_SelfMade")
        //.WithDescription("Retrieves a 5-day weather forecast with temperature and conditions")
        .Produces<IEnumerable<WeatherForecastResponse>>(StatusCodes.Status200OK);
    }
}
