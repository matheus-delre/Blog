using Domain.Abstractions.Messages;

namespace Application.Abstractions.Gateways;

public interface IEventBusGateway
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, IEvent;
}