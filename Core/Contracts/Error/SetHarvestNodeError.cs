using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct SetHarvestNodeError
    {
        public required SetHarvestNode SetHarvestNode { get; init; }
        public required BaseError BaseError { get; init; }
    }
}