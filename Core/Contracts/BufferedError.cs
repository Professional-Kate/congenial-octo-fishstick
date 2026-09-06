namespace IdelPog.Core.Contracts
{
    public readonly record struct BufferedError<T>
    {
        public required T[] Commands { get; init; }
        public required BaseError BaseError { get; init; }
    }
}