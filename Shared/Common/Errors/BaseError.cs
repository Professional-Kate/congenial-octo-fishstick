namespace IdelPog.Common.Errors
{
    public readonly record struct BaseError
    {
        public required Exception Exception { get; init; }
    }
}