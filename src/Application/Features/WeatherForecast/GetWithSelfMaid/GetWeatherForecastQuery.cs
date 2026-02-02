using Application.Abstractions.Messaging;

namespace Application.Features.WeatherForecast.GetWithSelfMaid;

/// <summary>
/// 天気予報取得クエリ
/// </summary>
/// <remarks>
internal sealed record WeatherForecastQuery : IQuery<IEnumerable<WeatherForecastResponse>>;
