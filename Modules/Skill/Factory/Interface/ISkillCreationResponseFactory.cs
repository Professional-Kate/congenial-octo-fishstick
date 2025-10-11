using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Response;

namespace IdelPog.Skill.Factory.Interface
{
    public interface ISkillCreationResponseFactory
    {
        public SkillCreationResponse[] Create(SkillCreation[] skillCreations);
    }
}