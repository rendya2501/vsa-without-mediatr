using FeatureWithMediatR.Behaivors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureWithMediatR;

/// <summary>
/// 依存性注入の設定クラス
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// FeatureWithMediatR の依存性注入の設定
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddFeatureWithMediatR(this IServiceCollection services)
    {
        // MediatR の登録
        services.AddMediatR(config =>
        {
            // ハンドラの自動登録
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // MediatR Pipeline Behaviors（実行順序: 登録順）
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        /// Pipeline Behaviors の手動登録の例
        /// MediatR v12以降、`AddOpenBehavior` が公式に推奨されている方法だが、それを知らなかった時代の備忘録として残す
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


        // ===================================================================
        // FluentValidationの登録
        // ===================================================================
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
