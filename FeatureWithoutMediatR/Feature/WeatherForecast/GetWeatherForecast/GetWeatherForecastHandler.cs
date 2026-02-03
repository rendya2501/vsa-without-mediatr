using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.WeatherForecast.GetWeatherForecast;

/// <summary>
/// クエリハンドラ（天気予報データ生成処理）
/// </summary>
/// <remarks>
/// 本クラスはデータベースやHTTPクライアントを必要としないため、
/// コンストラクタパラメータを持たない。
/// 本番環境では IWeatherService を注入して実装する想定。
/// </remarks>
internal sealed class GetWeatherForecastHandler 
    : IQueryHandler<WeatherForecastQuery, IEnumerable<WeatherForecastResponse>>
{
    /// <summary>
    /// 天気の状態を表す定数
    /// </summary>
    /// <remarks>
    /// 気温に応じた天気の表現を定義。
    /// 本番環境では外部APIからの応答をマッピングする際に使用。
    /// </remarks>
    private static readonly string[] Summaries =
    [
        "Freezing",    // 氷点下
        "Bracing",     // 身を切るような寒さ
        "Chilly",      // 肌寒い
        "Cool",        // 涼しい
        "Mild",        // 穏やか
        "Warm",        // 暖かい
        "Balmy",       // 快適
        "Hot",         // 暑い
        "Sweltering",  // うだるような暑さ
        "Scorching"    // 焼けつくような暑さ
    ];

    /// <summary>
    /// 5日間の天気予報データを生成
    /// </summary>
    /// <param name="_">天気予報取得クエリ</param>
    /// <param name="ct">キャンセルトークン（現在の実装では未使用）</param>
    /// <returns>5日分の天気予報データ</returns>
    public Task<IEnumerable<WeatherForecastResponse>> Handle(WeatherForecastQuery _, CancellationToken ct)
    {
        // 5日分の天気予報を生成
        var forecast = Enumerable.Range(1, 5).Select(index =>
        {
            // ランダムな摂氏温度を生成（-20°C ～ 55°C）
            var temperatureC = Random.Shared.Next(-20, 55);

            return new WeatherForecastResponse(
                Date: DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC: temperatureC,
                // 摂氏から華氏への変換: F = 32 + (C × 9/5)
                TemperatureF: 32 + (int)(temperatureC / 0.5556),
                // ランダムな天気概要を選択
                Summary: Summaries[Random.Shared.Next(Summaries.Length)]
            );
        });

        // 同期処理だが、IRequestHandlerのインターフェース要件を満たすためTask.FromResultで包む
        return Task.FromResult(forecast);
    }
}