using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeRequirementsCreation
    {
        public required SkillID SkillID { get; init; }
        public required HarvestNodeRequirement[] HarvestNodeRequirements { get; init; }
    }
}