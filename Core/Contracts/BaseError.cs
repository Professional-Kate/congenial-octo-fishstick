namespace IdelPog.Core.Contracts
{
    public readonly record struct BaseError
    {
        public required Exception Exception { get; init; }
    }
}