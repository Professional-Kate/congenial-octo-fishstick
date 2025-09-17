using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Service.Interface
{
    public interface ICurrentSkillSetter
    {
        public void SetCurrentSkill(SkillID skill);
    }
}