using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct HarvestNodeCreationResponse
    {
        public required ReadOnlyHarvestNode[] ReadOnlyHarvestNodes { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}