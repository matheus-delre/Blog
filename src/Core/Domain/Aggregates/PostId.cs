using Domain.Abstractions.Identities;

namespace Domain.Aggregates;

public record PostId : GuidIdentifier
{
    public PostId() { }
    public PostId(string value) : base(value) { }
    public PostId(Guid value) : base(value) { }

    public static PostId New => new();
    public static readonly PostId Undefined = new(Guid.Empty);

    public static explicit operator PostId(string value) => new(value);
    public override string ToString() => base.ToString();
}