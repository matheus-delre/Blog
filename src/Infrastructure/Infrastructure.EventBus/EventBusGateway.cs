using Application.Abstractions.Gateways;
using Domain.Abstractions.Messages;
using MassTransit;

namespace Infrastructure.EventBus;

public class EventBusGateway(IPublishEndpoint publishEndpoint) : IEventBusGateway
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, IEvent
        => publishEndpoint.Publish(@event, @event.GetType(), cancellationToken);
}