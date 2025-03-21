using Application.Abstractions;
using Application.Abstractions.Gateways;
using Domain.Abstractions.Aggregates;
using Domain.Abstractions.EventStore;
using Domain.Abstractions.Identities;
using Version = Domain.ValueObjects.Version;

namespace Application.Services;

public class ApplicationService(IEventStoreGateway eventStoreGateway, IEventBusGateway eventBusGateway, IUnitOfWork unitOfWork)
    : IApplicationService
{
    public async Task<TAggregate> LoadAggregateAsync<TAggregate, TId>(TId id, CancellationToken cancellationToken)
        where TAggregate : class, IAggregateRoot<TId>, new()
        where TId : IIdentifier, new()
    {
        var events = await eventStoreGateway.GetStreamAsync<TAggregate, TId>(id, Version.Zero, cancellationToken);

        if (events is { Count: 0 })
            throw new InvalidOperationException($"Aggregate {typeof(TAggregate).Name} with id {id} not found.");

        var aggregate = new TAggregate();
        aggregate.LoadFromStream(events);

        return aggregate;
    }

    public Task AppendEventsAsync<TAggregate, TId>(TAggregate aggregate, CancellationToken cancellationToken)
        where TAggregate : IAggregateRoot<TId>
        where TId : IIdentifier, new()
        => unitOfWork.ExecuteAsync(
            operationAsync: async ct =>
            {
                while (aggregate.TryDequeueEvent(out var @event))
                {
                    if (@event is null) continue;
                    var storeEvent = StoreEvent<TAggregate, TId>.Create(aggregate, @event);
                    await eventStoreGateway.AppendAsync(storeEvent, ct);
                    await eventBusGateway.PublishAsync(@event, ct);
                }
            },
            cancellationToken: cancellationToken);
}