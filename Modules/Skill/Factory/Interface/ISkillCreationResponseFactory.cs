using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Skill.Factory.Interface
{
    public interface ISkillCreationResponseFactory
    {
        public SkillCreationResponse[] Create(SkillCreation[] skillCreations);
    }
}