namespace IdelPog.Infrastructure.Structures
{
    public readonly struct Optional<T> where T : class
    {
        private readonly T _value;
        public bool HasValue { get; init; }
        
        public Optional(T value)
        {
            _value = value;
            HasValue = true;
        }

        public T GetValue()
        {
            return _value;
        }
        
        public static Optional<T> None => new();
    }
}