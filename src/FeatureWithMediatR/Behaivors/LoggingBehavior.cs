using DomainKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace FeatureWithMediatR.Behaivors;

/// <summary>
/// ロギングビヘイビアー
/// </summary>
/// <typeparam name="TRequest">MediatR の Request 型</typeparam>
/// <typeparam name="TResponse">MediatR の Response 型</typeparam>
/// <remarks>
/// <para>
/// MediatR の Pipeline Behavior として、すべての Request/Response を自動的にログに記録します。
/// ValidationBehavior と同様に、Handler の前後で実行されます。
/// </para>
/// <para>
/// <strong>構造化ログ（Serilog）:</strong><br/>
/// {@Object} 構文により、オブジェクトが構造化されたプロパティとして記録されます。
/// これにより、Seq、Elasticsearch、Splunk などのログ分析ツールで効率的に検索できます。
/// </para>
/// <para>
/// <strong>セキュリティ:</strong><br/>
/// Password、Secret、Token を含むプロパティは自動的にマスクされます。
/// </para>
/// <para>
/// <strong>分散トレーシング:</strong><br/>
/// リクエストごとに一意のGUIDを生成し、分散環境でのログ追跡を可能にします。
/// </para>
/// <para>
/// <strong>ログ出力例:</strong>
/// <code>
/// [14:23:45 INF] Request started: CreateGameCommand [a3f2b1c8] {...}
/// [14:23:45 INF] Request completed: CreateGameCommand [a3f2b1c8] in 45ms {...}
/// [14:23:46 ERR] Request failed: UpdateGameCommand [b4e3c2d9] in 120ms {...}
/// </code>
/// </para>
/// </remarks>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : Result
{
    /// <summary>
    /// MediatR パイプラインでの処理実行
    /// </summary>
    /// <param name="request">実際に送信された Command または Query</param>
    /// <param name="next">次の処理（ValidationBehavior → Handler）</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>Handler からの Response</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;                                                           
        var requestGuid = Guid.NewGuid().ToString("N")[..8];  // 短縮GUID (8文字)
        var sanitizedRequest = SanitizeRequest(request);
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "Request started: {RequestName} [{RequestGuid}] {@Request}",
            requestName,
            requestGuid,
            sanitizedRequest);

        TResponse response;

        try
        {
            // 次の処理を実行
            response = await next(cancellationToken);
            stopwatch.Stop();

            if (response.IsSuccess)
            {
                logger.LogInformation(
                    "Request completed: {RequestName} [{RequestGuid}] in {ElapsedMilliseconds}ms {@Response}",
                    requestName,
                    requestGuid,
                    stopwatch.ElapsedMilliseconds,
                    response);
            }
            else
            {
                using (LogContext.PushProperty("Error", response.Error, true))
                {
                    logger.LogError(
                        "Request error: {RequestName} [{RequestGuid}] in {ElapsedMilliseconds}ms {@Response}",
                        requestName,
                        requestGuid,
                        stopwatch.ElapsedMilliseconds,
                        response);
                }
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // リクエスト失敗ログ（ValidationException も含む）
            // Serilogは例外オブジェクトを自動的に構造化して記録
            logger.LogError(
                ex,
                "Request error: {RequestName} [{RequestGuid}] after {ElapsedMilliseconds}ms",
                requestName,
                requestGuid,
                stopwatch.ElapsedMilliseconds);

            // 例外は再スローして、ExceptionHandler で処理させる
            throw;
        }

        return response;
    }

    /// <summary>
    /// 機密情報を含むプロパティをマスク
    /// </summary>
    /// <param name="request">リクエストオブジェクト</param>
    /// <returns>マスク処理されたプロパティのディクショナリ</returns>
    /// <remarks>
    /// Password、Secret、Token を名前に含むプロパティは "***REDACTED***" に置き換えられます。
    /// </remarks>
    private static Dictionary<string, object?> SanitizeRequest(TRequest request)
    {
        var properties = typeof(TRequest).GetProperties()
            .Select(p => new
            {
                Name = p.Name,
                Value = p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Contains("Token", StringComparison.OrdinalIgnoreCase)
                    ? "***REDACTED***"
                    : p.GetValue(request)
            })
            .ToDictionary(x => x.Name, x => x.Value);

        return properties;
    }
}
