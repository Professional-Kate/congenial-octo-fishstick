namespace IdelPog.Common.Structures
{
    public readonly struct Optional<T>
    {
        private readonly T _value;
        public bool HasValue { get; }
        
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