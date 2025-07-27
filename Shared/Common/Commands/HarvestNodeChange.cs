using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct HarvestNodeChange
    {
        public required ResourceID ResourceID { get; init; }
    }
}