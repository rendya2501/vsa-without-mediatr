using DomainKernel;
using MediatR;

namespace FeatureWithMediatR.Features.WeatherForecast.GetWeatherForecast;

/// <summary>
/// 天気予報取得クエリ
/// </summary>
/// <remarks>
internal sealed record WeatherForecastQuery 
    : IRequest<Result<IEnumerable<WeatherForecastResponse>>>;
