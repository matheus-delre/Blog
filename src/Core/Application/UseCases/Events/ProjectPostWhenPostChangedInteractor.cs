using Application.Abstractions;
using Application.Abstractions.Gateways;
using Domain.Aggregates.Events;
using Domain.Aggregates.Projections;

namespace Application.UseCases.Events;

public interface IProjectPostWhenPostChangedInteractor : IInteractor<DomainEvent.PostCreated>;

public class ProjectPostWhenPostChangedInteractor(
    IProjectionGateway<Projection.Post> projectionGateway) : IProjectPostWhenPostChangedInteractor
{
    public async Task InteractAsync(DomainEvent.PostCreated @event, CancellationToken cancellationToken)
        => await projectionGateway.ReplaceInsertAsync(new(@event.PostId, @event.Title, @event.Content), cancellationToken);
}