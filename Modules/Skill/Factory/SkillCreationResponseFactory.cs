using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public sealed class SkillCreationResponseFactory : ISkillCreationResponseFactory
    {
        public SkillCreationResponse Create(SkillCreation[] skillCreations)
        {
            return new SkillCreationResponse
            {
                SkillCreations = skillCreations
            };
        }
    }
}