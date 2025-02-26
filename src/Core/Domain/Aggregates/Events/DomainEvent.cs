using Domain.Abstractions.Messages;

namespace Domain.Aggregates.Events;

public static class DomainEvent
{
    public record PostCreated(string PostId, string Title, string Content, ulong Version) : Message, IDomainEvent;
    public record CommentAdded(string CommentId, string PostId, string Content, ulong Version) : Message, IDomainEvent;
}