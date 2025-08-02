using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct SetHarvestNodeError
    {
        public required SetHarvestNode SetHarvestNode { get; init; }
        public required BaseError BaseError { get; init; }
    }
}