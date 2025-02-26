using Application.UseCases.Events;
using Domain.Aggregates.Events;
using MassTransit;

namespace Infrastructure.EventBus.Consumers
{
    public class ProjectPostWhenPostChangedConsumer(IProjectPostWhenPostChangedInteractor interactor)
        : IConsumer<DomainEvent.PostCreated>
    {
        public Task Consume(ConsumeContext<DomainEvent.PostCreated> context)
            => interactor.InteractAsync(context.Message, context.CancellationToken);
    }
}
