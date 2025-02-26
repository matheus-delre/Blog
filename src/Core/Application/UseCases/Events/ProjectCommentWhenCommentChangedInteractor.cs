using Application.Abstractions;
using Application.Abstractions.Gateways;
using Domain.Aggregates.Events;
using Domain.Aggregates.Projections;

namespace Application.UseCases.Events;

public interface IProjectCommentWhenCommentChangedInteractor : IInteractor<DomainEvent.CommentAdded>;

public class ProjectCommentWhenCommentChangedInteractor(
    IProjectionGateway<Projection.Comment> projectionGateway) : IProjectCommentWhenCommentChangedInteractor
{
    public async Task InteractAsync(DomainEvent.CommentAdded @event, CancellationToken cancellationToken)
        => await projectionGateway.ReplaceInsertAsync(new(@event.CommentId, @event.PostId, @event.Content), cancellationToken);
}