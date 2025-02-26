using Application.Abstractions.Gateways;
using Domain.Abstractions.Messages;
using Infrastructure.EventBus.DependencyInjection.Options;
using Infrastructure.EventBus.PipeFilters;
using Infrastructure.EventBus.PipeObservers;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Infrastructure.EventBus.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
        => services
            .ConfigureOptions()
            .AddEventBusGateway()
            .AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddConsumers(Assembly.GetExecutingAssembly());

                cfg.UsingRabbitMq((context, bus) =>
                {
                    var options = context.GetRequiredService<IOptions<MessageBusOptions>>().Value;

                    bus.Host(
                        hostAddress: options.ConnectionString,
                        connectionName: $"{options.ConnectionName}.{AppDomain.CurrentDomain.FriendlyName}");

                    bus.UseNewtonsoftJsonSerializer();

                    bus.MessageTopology.SetEntityNameFormatter(new KebabCaseEntityNameFormatter());

                    bus.ConnectReceiveObserver(new LoggingReceiveObserver());
                    bus.ConnectConsumeObserver(new LoggingConsumeObserver());
                    bus.ConnectPublishObserver(new LoggingPublishObserver());
                    bus.ConnectSendObserver(new LoggingSendObserver());

                    bus.UsePublishFilter(typeof(TraceIdentifierFilter<>), context);

                    bus.ConfigureEndpoints(context);

                    bus.ConfigurePublish(pipe => pipe.AddPipeSpecification(
                        new DelegatePipeSpecification<PublishContext<IEvent>>(ctx
                            => ctx.CorrelationId = ctx.InitiatorId)));
                });
            });

    private static IServiceCollection AddEventBusGateway(this IServiceCollection services)
        => services.AddScoped<IEventBusGateway, EventBusGateway>();

    private static IServiceCollection ConfigureOptions(this IServiceCollection services)
        => services
            .ConfigureOptions<MessageBusOptions>()
            .ConfigureOptions<MassTransitHostOptions>();

    private static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services)
        where TOptions : class
        => services
            .AddOptions<TOptions>()
            .BindConfiguration(typeof(TOptions).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
}