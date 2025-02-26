namespace Domain.ValueObjects
{
    public record Title
    {
        private readonly string _value;

        public Title(string title)
        {
            ArgumentException.ThrowIfNullOrEmpty(title);
            ArgumentOutOfRangeException.ThrowIfLessThan(title.Length, 5);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(title.Length, 50);

            _value = title;
        }

        public static Title Undefined => new("Undefined");
        public static explicit operator Title(string title) => new(title);
        public static implicit operator string(Title title) => title._value;
        public override string ToString() => _value;
    }
}
