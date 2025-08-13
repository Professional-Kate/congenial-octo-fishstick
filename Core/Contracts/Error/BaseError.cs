namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct BaseError
    {
        public required Exception Exception { get; init; }
    }
}