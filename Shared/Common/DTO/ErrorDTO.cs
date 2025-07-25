namespace IdelPog.Common.DTO
{
    public readonly record struct ErrorDTO
    {
        public required Exception Exception { get; init; }
    }
}