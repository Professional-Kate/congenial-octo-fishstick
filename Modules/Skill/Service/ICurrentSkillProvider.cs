using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Service
{
    public interface ICurrentSkillProvider
    {
        public SkillID GetCurrentSkill();
    }
}