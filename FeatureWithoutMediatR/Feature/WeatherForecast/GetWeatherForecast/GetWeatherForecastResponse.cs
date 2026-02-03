namespace FeatureWithoutMediatR.Feature.WeatherForecast.GetWeatherForecast;

/// <summary>
/// 天気予報のレスポンスDTO
/// </summary>
/// <param name="Date">予報日</param>
/// <param name="TemperatureC">摂氏温度</param>
/// <param name="TemperatureF">華氏温度</param>
/// <param name="Summary">天気の概要（Freezing, Hot など）</param>
internal sealed record WeatherForecastResponse(
    DateOnly Date,
    int TemperatureC,
    int TemperatureF,
    string? Summary);
