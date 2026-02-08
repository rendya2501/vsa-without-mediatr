using Infrastructure.Database;
using Infrastructure.Database.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// 依存性注入の設定クラス
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure の依存性注入の設定
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <param name="environment">IHostEnvironment</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // ===================================================================
        // DbContext の登録
        // ===================================================================
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (environment.IsDevelopment())
            {
                options.UseInMemoryDatabase("GameDB");
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            }
        });


        // ===================================================================
        // DBシーダーの登録
        // ===================================================================
        services.AddScoped<IDbSeeder, ApplicationDbSeeder>();

        return services;
    }
}
