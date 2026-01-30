using Serilog;

namespace Web.Api.Extensions;

/// <summary>
/// Serilog設定を整理する拡張メソッド
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Serilogの本設定を適用
    /// </summary>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
    {
        // appsettings.json の設定を読み込んでロガーを再構成
        // これ以降のログはすべて appsettings.json の設定に従う
        host.UseSerilog((context, services, configuration) => configuration
            // appsettings.json の "Serilog" セクションから設定を読み込み
            .ReadFrom.Configuration(context.Configuration)
            // DI コンテナから設定を読み込み（今回は未使用だが拡張性のため）
            .ReadFrom.Services(services)
            // LogContext からプロパティを追加（リクエストごとの追加情報に使用）
            .Enrich.FromLogContext()
            // マシン名をすべてのログに追加（分散環境で便利）
            .Enrich.WithMachineName()
            // スレッドIDをすべてのログに追加（マルチスレッド解析に便利）
            .Enrich.WithThreadId()
            // カスタムプロパティを追加（すべてのログに Application = "VideoGameApiVsa" が付く）
            .Enrich.WithProperty("Application", "VideoGameApiVsa")
        );

        return host;
    }
}
