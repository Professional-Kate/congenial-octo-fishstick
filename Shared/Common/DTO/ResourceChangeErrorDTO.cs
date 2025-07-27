namespace IdelPog.Common.DTO
{
    public readonly record struct ResourceChangeErrorDTO
    {
        public required ResourceChangeDTO ResourceChangeDTO { get; init; }
        public required ErrorDTO ErrorDTO { get; init; }
    }
}