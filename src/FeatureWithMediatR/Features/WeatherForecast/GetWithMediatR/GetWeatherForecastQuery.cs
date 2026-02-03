using MediatR;

namespace FeatureWithMediatR.Features.WeatherForecast.GetWithMediatR;

/// <summary>
/// 天気予報取得クエリ
/// </summary>
/// <remarks>
internal sealed record WeatherForecastQuery : IRequest<IEnumerable<WeatherForecastResponse>>;
