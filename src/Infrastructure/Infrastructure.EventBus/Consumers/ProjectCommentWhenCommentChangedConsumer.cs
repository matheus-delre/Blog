using Application.UseCases.Events;
using Domain.Aggregates.Events;
using MassTransit;

namespace Infrastructure.EventBus.Consumers
{

    public class ProjectCommentWhenCommentChangedConsumer(IProjectCommentWhenCommentChangedInteractor interactor)
        : IConsumer<DomainEvent.CommentAdded>
    {
        public Task Consume(ConsumeContext<DomainEvent.CommentAdded> context)
            => interactor.InteractAsync(context.Message, context.CancellationToken);
    }
}
