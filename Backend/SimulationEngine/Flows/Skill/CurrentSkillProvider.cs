using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
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