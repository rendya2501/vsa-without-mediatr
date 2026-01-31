using Scalar.AspNetCore;
using Serilog;
using Web.Api.Extensions;

namespace Web.Api.Extensions;

/// <summary>
/// ミドルウェアパイプラインの設定を整理する拡張メソッド
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Serilogのリクエストロギング設定
    /// </summary>
    public static WebApplication UseSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });

        return app;
    }
}
