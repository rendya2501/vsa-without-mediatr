using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithMediatR.Features.WeatherForecast.GetWithMediatR;

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
        app.MapGet("/api/weather-forecast-mediatr/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new WeatherForecastQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithTags("WithMediatR")
        //.WithName("GetWeatherForecast_MediatR")
        //.WithDescription("Retrieves a 5-day weather forecast with temperature and conditions")
        .Produces<IEnumerable<WeatherForecastResponse>>(StatusCodes.Status200OK);
    }
}
