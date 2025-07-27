using IdelPog.Common.Enums;

namespace IdelPog.Common.DTO
{
    public readonly record struct ResourceChangeDTO
    {
        public required ResourceID ResourceID { get; init; }
    }
}