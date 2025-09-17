using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct SkillCreationResponse
    {
        public required SkillCreation[] SkillCreations { get; init; }
    }
}