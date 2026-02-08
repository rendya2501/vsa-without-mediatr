using Carter;
using Serilog;
using Web.Api.ExceptionHandlers;

namespace Web.Api;

/// <summary>
/// 依存性注入の設定クラス
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Web.Api の依存性注入の設定
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // ===================================================================
        // OpenAPI
        // ===================================================================
        services.AddEndpointsApiExplorer(); // APIエンドポイントの情報を探索可能にする
        services.AddOpenApi();


        // ===================================================================
        // 例外ハンドラー（順序が重要！）
        // ===================================================================
        // 特定の例外を先に登録
        services.AddExceptionHandler<ValidationExceptionHandler>();
        // グローバルハンドラーを最後に登録
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();


        // ===================================================================
        // Carter（Minimal API拡張）
        // ===================================================================
        services.AddCarter();

        return services;
    }


    /// <summary>
    /// Serilog の設定を適用
    /// </summary>
    /// <param name="host">IHostBuilder</param>
    /// <returns>IHostBuilder</returns>
    /// <remarks>
    /// appsettings.json の "Serilog" セクションから設定を読み込み、
    /// 構造化ログを有効化します。
    /// </remarks>
    public static IHostBuilder AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) => configuration
            // appsettings.json の "Serilog" セクションから設定を読み込み
            .ReadFrom.Configuration(context.Configuration)
            // DI コンテナから設定を読み込み（拡張性のため）
            .ReadFrom.Services(services)
            // LogContext からプロパティを追加（リクエストごとの追加情報に使用）
            .Enrich.FromLogContext()
            // マシン名をすべてのログに追加（分散環境で便利）
            .Enrich.WithMachineName()
            // スレッドIDをすべてのログに追加（マルチスレッド解析に便利）
            .Enrich.WithThreadId()
            // カスタムプロパティを追加
            .Enrich.WithProperty("Application", "VsaWithoutMediatR")
        );

        return host;
    }

    /// <summary>
    /// Serilog のリクエストロギングを設定
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication</returns>
    /// <remarks>
    /// HTTP リクエスト/レスポンスのログを構造化された形式で記録します。
    /// </remarks>
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
