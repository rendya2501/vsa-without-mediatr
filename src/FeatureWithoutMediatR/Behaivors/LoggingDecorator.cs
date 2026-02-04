using DomainKernel;
using FeatureShared.Messaging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FeatureWithoutMediatR.Behaivors;

internal static class LoggingDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

//public class LoggingDecorator<TRequest, TResponse>(
//    ILogger<LoggingDecorator<TRequest, TResponse>> logger)
//    : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : class
//    where TResponse : Result
//{
//    public async Task<TResponse> Handle(
//        TRequest request,
//        RequestHandlerDelegate<TResponse> next,
//        CancellationToken cancellationToken)
//    {
//        // リクエスト名
//        var requestName = typeof(TRequest).Name;
//        // 短縮GUID (8文字)                                                          
//        var requestGuid = Guid.NewGuid().ToString("N")[..8];
//        // 機密情報をマスク
//        var sanitizedRequest = SanitizeRequest(request);
//        // 処理時間を計測
//        var stopwatch = Stopwatch.StartNew();

//        // リクエスト開始ログ
//        // {@Request} で構造化ログとして記録(Serilogの機能)
//        if (logger.IsEnabled(LogLevel.Information))
//        {
//            logger.LogInformation(
//                "Handling {RequestName} [{RequestGuid}] {@Request}",
//                requestName,
//                requestGuid,
//                sanitizedRequest);
//        }

//        TResponse response;

//        try
//        {
//            // 次の処理を実行（ValidationBehavior → Handler）
//            response = await next(cancellationToken);

//            stopwatch.Stop();

//            // リクエスト成功ログ
//            // {@Response} で構造化ログとして記録
//            if (logger.IsEnabled(LogLevel.Information))
//            {
//                logger.LogInformation(
//                    "Handled {RequestName} [{RequestGuid}] in {ElapsedMilliseconds}ms {@Response}",
//                    requestName,
//                    requestGuid,
//                    stopwatch.ElapsedMilliseconds,
//                    response);
//            }
//        }
//        catch (Exception ex)
//        {
//            stopwatch.Stop();

//            // リクエスト失敗ログ（ValidationException も含む）
//            // Serilogは例外オブジェクトを自動的に構造化して記録
//            logger.LogError(
//                ex,
//                "Error handling {RequestName} [{RequestGuid}] after {ElapsedMilliseconds}ms",
//                requestName,
//                requestGuid,
//                stopwatch.ElapsedMilliseconds);

//            // 例外は再スローして、ExceptionHandler で処理させる
//            throw;
//        }

//        return response;
//    }

//    /// <summary>
//    /// リクエスト内容をマスク
//    /// </summary>
//    /// <param name="request">リクエスト</param>
//    /// <returns>マスクされたリクエスト内容</returns>
//    private static Dictionary<string, object?> SanitizeRequest(TRequest request)
//    {
//        // プロパティ名に "Password", "Secret", "Token" が含まれる場合はマスク
//        var properties = typeof(TRequest).GetProperties()
//            .Select(p => new
//            {
//                Name = p.Name,
//                Value = p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
//                        p.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
//                        p.Name.Contains("Token", StringComparison.OrdinalIgnoreCase)
//                    ? "***REDACTED***"
//                    : p.GetValue(request)
//            })
//            .ToDictionary(x => x.Name, x => x.Value);

//        return properties;
//    }
//}
