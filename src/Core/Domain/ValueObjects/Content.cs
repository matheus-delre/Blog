namespace Domain.ValueObjects
{
    public record Content
    {
        private readonly string _value;

        public Content(string content)
        {
            ArgumentException.ThrowIfNullOrEmpty(content);
            ArgumentOutOfRangeException.ThrowIfLessThan(content.Length, 5);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(content.Length, 240);

            _value = content;
        }

        public static Content Undefined => new("Undefined");
        public static explicit operator Content(string content) => new(content);
        public static implicit operator string(Content content) => content._value;
        public override string ToString() => _value;
    }
}
