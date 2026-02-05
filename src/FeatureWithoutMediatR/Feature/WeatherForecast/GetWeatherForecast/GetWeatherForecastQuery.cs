using FeatureShared.Messaging;

namespace FeatureWithoutMediatR.Feature.WeatherForecast.GetWeatherForecast;

/// <summary>
/// 天気予報取得クエリ
/// </summary>
/// <remarks>
public sealed record WeatherForecastQuery : IQuery<IEnumerable<WeatherForecastResponse>>;
