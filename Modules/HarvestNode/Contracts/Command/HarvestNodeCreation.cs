using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Command
{
    public readonly record struct HarvestNodeCreation
    {
        public required ReadOnlyHarvestNode[] ReadOnlyHarvestNodes { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}