using FeatureShared.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureWithoutMediatR;

public static class DependencyInjection
{
    public static IServiceCollection AddFeatureWithoutMediatR(this IServiceCollection services)
    {
        // 自動登録の例
        // ①
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
        // ②
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
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



        //services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        //services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        //services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        //services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        //services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        //services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
        //    .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
        //    .AsImplementedInterfaces()
        //    .WithScopedLifetime());


        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
