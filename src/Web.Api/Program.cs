using Carter;
using FeatureWithMediatR;
using FeatureWithoutMediatR;
using Infrastructure.Database;
using Infrastructure.Database.Seeding;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Web.Api.ExceptionHandlers;
using Web.Api.Extensions;

// ===================================================================
// Serilog の初期設定（ブートストラップロガー）
// ===================================================================
// この時点では appsettings.json がまだ読み込まれていないため、
// 最低限のロガーを作成（アプリケーション起動時のエラーもキャッチ可能）
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting VideoGameApiVsa application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 拡張メソッドでSerilog設定
    builder.Host.ConfigureSerilog();

    builder.Services.AddOpenApi();

    builder.Services
        .AddFeatureWithMediatR()
        .AddFeatureWithoutMediatR();

    // DbContext（In-Memory Database）
    builder.Services.AddDbContext<VideoGameDbContext>(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.UseInMemoryDatabase("GameDB");
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        }
    });

    // DBシーダー登録
    builder.Services.AddScoped<IDbSeeder, VideoGameDbSeeder>();

    // ===================================================================
    // 例外ハンドラー（順序が重要！）
    // ===================================================================
    // 特定の例外を先に登録
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    // グローバルハンドラーを最後に登録
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Carter（Minimal API拡張）
    builder.Services.AddCarter();

    var app = builder.Build();

    // ミドルウェアパイプライン（順序が重要）
    app.UseSerilogRequestLogging();  // HTTPリクエストロギング

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        //using var scope = app.Services.CreateScope();
        //var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
        //await seeder.SeedAsync();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VideoGameDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    app.UseHttpsRedirection();

    // 例外ハンドリング
    app.UseExceptionHandler();

    // エンドポイント登録
    app.MapCarter();

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
