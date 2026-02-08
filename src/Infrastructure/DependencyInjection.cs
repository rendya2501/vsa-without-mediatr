using Infrastructure.Database;
using Infrastructure.Database.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Infrastructure レイヤーの依存性注入設定
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure レイヤーのサービスを登録
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <param name="configuration">アプリケーション設定</param>
    /// <param name="environment">ホスト環境情報</param>
    /// <returns>更新されたサービスコレクション</returns>
    /// <remarks>
    /// <para>
    /// 環境に応じて異なるデータベースプロバイダーを構成します:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <strong>開発環境:</strong> InMemory データベース
    /// - メリット: セットアップ不要、高速、テストに最適
    /// - デメリット: データは永続化されない（再起動で消失）
    /// </item>
    /// <item>
    /// <strong>本番環境:</strong> SQL Server
    /// - メリット: データ永続化、トランザクション保証、スケーラビリティ
    /// - 要件: appsettings.json に "DefaultConnection" 接続文字列が必要
    /// </item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // ===================================================================
        // DbContext の登録
        // ===================================================================
        // 環境に応じてデータベースプロバイダーを切り替え
        // - 開発環境: InMemory（セットアップ不要、テスト向け）
        // - 本番環境: SQL Server（データ永続化、本番運用向け）
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (environment.IsDevelopment())
            {
                // InMemory データベース
                // メリット: セットアップ不要、高速、テストに最適
                // 注意: アプリケーション再起動でデータが失われます
                options.UseInMemoryDatabase("GameDB");
            }
            else
            {
                // SQL Server データベース
                // 要件: appsettings.json に "ConnectionStrings:DefaultConnection" が必要
                // 例: "Server=localhost;Database=GameDB;Trusted_Connection=True;"
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            }
        });


        // ===================================================================
        // DBシーダーの登録
        // ===================================================================
        // データベース初期化とシードデータ投入を担当
        // Program.cs で手動実行するか、マイグレーション時に自動実行可能
        services.AddScoped<IDbSeeder, ApplicationDbSeeder>();

        return services;
    }
}
