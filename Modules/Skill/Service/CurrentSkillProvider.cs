using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Service
{
    public class CurrentSkillProvider : ICurrentSkillProvider, ICurrentSkillSetter
    {
        private SkillID _currentSkill;

        public SkillID GetCurrentSkill()
        {
            return _currentSkill;
        }

        public void SetCurrentSkill(SkillID skill)
        {
            _currentSkill = skill;
        }
    }
}