using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct SetHarvestNodeResponse
    {
        public required SetHarvestNode SetHarvestNode { get; init; }
    }
}