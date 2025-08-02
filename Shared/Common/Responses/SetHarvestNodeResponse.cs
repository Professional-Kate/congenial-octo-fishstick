using IdelPog.Common.Commands;

namespace IdelPog.Common.Responses
{
    public readonly record struct SetHarvestNodeResponse
    {
        public required SetHarvestNode SetHarvestNode { get; init; }
    }
}