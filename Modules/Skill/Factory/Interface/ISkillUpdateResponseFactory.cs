using IdelPog.Skill.Contracts.Response;

namespace IdelPog.Skill.Factory.Interface
{
    public interface ISkillUpdateResponseFactory
    {
        public SkillUpdateResponse Create(Contracts.Skill skill, bool hasLeveled);
    }
}