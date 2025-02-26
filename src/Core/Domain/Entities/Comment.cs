using Domain.Abstractions.Entities;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Comment : Entity<CommentId>
    {
        public Comment(CommentId id, Content content)
        {
            Id = id;
            Content = content;
        }
        public Content Content { get; private set; }

        public static Comment Create(CommentId commentId, Content content)
            => new(commentId, content);
    }
}
