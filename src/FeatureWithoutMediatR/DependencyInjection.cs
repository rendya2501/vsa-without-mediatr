using FeatureShared.Messaging;
using FeatureWithoutMediatR.Behaivors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureWithoutMediatR;

/// <summary>
/// 依存性注入の設定クラス
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// FeatureWithoutMediatR の依存性注入の設定
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddFeatureWithoutMediatR(this IServiceCollection services)
    {
        // ===================================================================
        // ハンドラーの登録
        // ===================================================================
        // 自動登録の例
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        // 手動登録の例
        // ハンドラーを追加する度にここに追記する必要があるため、自動登録を推奨
        // services.AddScoped<IQueryHandler<WeatherForecastQuery, IEnumerable<WeatherForecastResponse>>, GetWeatherForecastHandler>();

        // 自動登録の例その2
        // メリット
        // - 1ブロックで完結していて短い
        // - インターフェース追加時にコードを触らなくて済む
        // デメリット
        // - 継承階層・複数実装・将来のIF追加で挙動が読みにくい
        // -「なぜこの条件なのか」がコードから直感的に分かりづらい
        // - CQRSのIFが増えたときに 巨大な || 地獄になる
        // まとめ
        // Scrutorの正規ルート AssignableTo を素直に使う方が良い。
        //services.Scan(scan => scan
        //    .FromAssembliesOf(typeof(DependencyInjection))
        //    .AddClasses(classes => classes.Where(type =>
        //        type.GetInterfaces().Any(@interface =>
        //            @interface.IsGenericType &&
        //            (@interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
        //             @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
        //             @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))),
        //        publicOnly: false)
        //    .AsImplementedInterfaces()
        //    .WithScopedLifetime());


        // =================================================================== 
        // デコレーターの登録
        // ===================================================================
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));


        // =================================================================== 
        // ドメインイベントハンドラーの登録
        // ===================================================================
        //services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
        //    .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
        //    .AsImplementedInterfaces()
        //    .WithScopedLifetime());


        // ===================================================================
        // FluentValidationの登録
        // ===================================================================
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
