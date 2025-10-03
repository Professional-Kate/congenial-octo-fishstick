using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Service.Interface
{
    public interface ICurrentSkillProvider
    {
        public SkillID GetCurrentSkill();
    }
}