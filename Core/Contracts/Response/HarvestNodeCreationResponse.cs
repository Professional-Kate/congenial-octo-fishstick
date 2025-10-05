using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeCreationResponse
    {
        public required ReadOnlyHarvestNode[] ReadOnlyHarvestNodes { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}