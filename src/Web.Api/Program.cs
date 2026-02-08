using Carter;
using FeatureWithMediatR;
using FeatureWithoutMediatR;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Web.Api;

// ===================================================================
// Serilog の初期設定（ブートストラップロガー）
// ===================================================================
// この時点では appsettings.json がまだ読み込まれていないため、
// 最低限のロガーを作成（アプリケーション起動時のエラーもキャッチ可能）
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting VsaWithoutMediatR application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ===================================================================
    // 各レイヤーの依存性注入を設定
    // ==================================================================
    // Serilog設定
    builder.Host.AddSerilog();

    // 各レイヤーのDI設定
    builder.Services
        .AddFeatureWithMediatR()
        .AddFeatureWithoutMediatR()
        .AddPresentation()
        .AddInfrastructure(builder.Configuration, builder.Environment);

    var app = builder.Build();

    // ===================================================================
    // ミドルウェアパイプライン（順序が重要）
    // ===================================================================
    // Serilog リクエストロギング
    app.UseSerilogRequestLogging();

    // 開発環境専用設定
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    // HTTPSリダイレクト
    app.UseHttpsRedirection();

    // 例外ハンドリング
    app.UseExceptionHandler();
    
    // Carterのエンドポイント登録
    app.MapCarter();

    // アプリケーションの起動
    Log.Information("Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    // アプリケーション起動時の致命的エラー
    // appsettings.json 読み込み失敗などもここでキャッチ
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // アプリケーション終了時に確実にログをフラッシュ
    // バッファに残っているログをすべて書き込んでからプロセスを終了
    Log.CloseAndFlush();
}
