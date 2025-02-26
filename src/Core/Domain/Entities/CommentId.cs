using Domain.Abstractions.Identities;

namespace Domain.Aggregates;

public record CommentId : GuidIdentifier
{
    public CommentId() { }
    public CommentId(string value) : base(value) { }
    public CommentId(Guid value) : base(value) { }

    public static CommentId New => new();
    public static readonly CommentId Undefined = new(Guid.Empty);

    public static explicit operator CommentId(string value) => new(value);
    public override string ToString() => base.ToString();
}