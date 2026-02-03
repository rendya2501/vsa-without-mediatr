using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FeatureWithMediatR.Behaivors;

/// <summary>
/// MediatR の Pipeline Behavior（Serilogロギング処理）
/// </summary>
/// <typeparam name="TRequest">MediatR の Request 型</typeparam>
/// <typeparam name="TResponse">MediatR の Response 型</typeparam>
/// <remarks>
/// <para>
/// すべての Request/Response を自動的にログに記録する横断的関心事。
/// ValidationBehavior と同様に、Handler の前後で実行される。
/// </para>
/// <para>
/// <strong>処理フロー:</strong><br/>
/// 1. リクエスト開始時刻とGUIDを記録<br/>
/// 2. リクエスト内容を構造化ログとして記録<br/>
/// 3. 次の処理（Validation → Handler）を実行<br/>
/// 4. レスポンス内容と実行時間を記録<br/>
/// 5. エラー発生時は例外情報を記録
/// </para>
/// <para>
/// <strong>Serilogの構造化ログ:</strong><br/>
/// {@Request} のように @ を使用することで、
/// オブジェクトが構造化されたプロパティとしてログに記録される。
/// これにより、Seq、Elasticsearch、Splunk などで検索可能になる。
/// </para>
/// <para>
/// <strong>ログ出力例:</strong><br/>
/// <code>
/// [14:23:45 INF] Handling CreateGameCommand [a3f2b1c8] {@Request}
/// [14:23:45 INF] Handled CreateGameCommand [a3f2b1c8] in 45ms {@Response}
/// </code>
/// </para>
/// </remarks>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// MediatR パイプラインでの処理実行
    /// </summary>
    /// <param name="request">実際に送信された Command / Query</param>
    /// <param name="next">次の処理（次の Behavior or 最終的な Handler）</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>Handler からの Response</returns>
    /// <remarks>
    /// <para>
    /// リクエスト単位でGUIDを生成することで、
    /// 分散ログ環境でも同一リクエストのログを追跡可能にする。
    /// </para>
    /// <para>
    /// Serilogの構造化ログを活用し、{@Request} / {@Response} として
    /// オブジェクト全体を記録することで、Seq等での高度な検索を実現。
    /// </para>
    /// </remarks>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString("N")[..8]; // 短縮GUID (8文字)
        var stopwatch = Stopwatch.StartNew();

        // リクエスト開始ログ
        // {@Request} で構造化ログとして記録(Serilogの機能)
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Handling {RequestName} [{RequestGuid}] {@Request}",
                requestName,
                requestGuid,
                request);
        }

        TResponse response;

        try
        {
            // 次の処理を実行（ValidationBehavior → Handler）
            response = await next(ct);

            stopwatch.Stop();

            // リクエスト成功ログ
            // {@Response} で構造化ログとして記録
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Handled {RequestName} [{RequestGuid}] in {ElapsedMilliseconds}ms {@Response}",
                    requestName,
                    requestGuid,
                    stopwatch.ElapsedMilliseconds,
                    response);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // リクエスト失敗ログ（ValidationException も含む）
            // Serilogは例外オブジェクトを自動的に構造化して記録
            logger.LogError(
                ex,
                "Error handling {RequestName} [{RequestGuid}] after {ElapsedMilliseconds}ms",
                requestName,
                requestGuid,
                stopwatch.ElapsedMilliseconds);

            // 例外は再スローして、ExceptionHandler で処理させる
            throw;
        }

        return response;
    }
}
