namespace IdelPog.Common.DTO.Error
{
    public readonly record struct ErrorDTO
    {
        public required Exception Exception { get; init; }
    }
}