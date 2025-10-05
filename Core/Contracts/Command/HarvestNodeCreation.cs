using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeCreation
    {
        public required ReadOnlyHarvestNode[] ReadOnlyHarvestNodes { get; init; }
        public required SkillID LinkedSkill { get; init; }
    }
}