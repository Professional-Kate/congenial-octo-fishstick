using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct HarvestNodeRequirementsCreationResponse
    { 
        public required SkillID SkillID { get; init; }
        public required HarvestNodeRequirement[] HarvestNodeRequirements { get; init; }
    }
}