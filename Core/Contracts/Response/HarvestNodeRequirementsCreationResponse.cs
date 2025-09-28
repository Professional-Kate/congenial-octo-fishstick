using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeRequirementsCreationResponse
    { 
        public required SkillID SkillID { get; init; }
        public required HarvestNodeRequirement[] HarvestNodeRequirements { get; init; }
    }
}