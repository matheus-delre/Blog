using Domain.Abstractions.Aggregates;
using Domain.Abstractions.Messages;
using Domain.Aggregates.Events;
using Domain.Entities;
using Domain.ValueObjects;
using Newtonsoft.Json;
using Version = Domain.ValueObjects.Version;

namespace Domain.Aggregates;

public class Post : AggregateRoot<PostId>
{
    [JsonProperty]
    private readonly List<Comment> _comments = [];

    public Title Title { get; private set; } = Title.Undefined;
    public Content Content { get; private set; } = Content.Undefined;
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();

    public static Post Create(Title title, Content content)
    {
        Post post = new();

        DomainEvent.PostCreated @event = new(post.Id, title, content, Version.Initial);

        post.RaiseEvent(@event);

        return post;
    }

    public void AddComment(Content comment)
        => RaiseEvent(new DomainEvent.CommentAdded(CommentId.New, Id, comment, Version.Next));

    protected override void ApplyEvent(IDomainEvent @event)
        => When(@event as dynamic);

    private void When(DomainEvent.PostCreated @event)
    {
        Id = (PostId)@event.PostId;
        Title = (Title)@event.Title;
        Content = (Content)@event.Content;
        Version = (Version)@event.Version;
    }

    private void When(DomainEvent.CommentAdded @event)
    {
        _comments.Add(Comment.Create((CommentId)@event.CommentId, (Content)@event.Content));
        Version = (Version)@event.Version;
    }
}