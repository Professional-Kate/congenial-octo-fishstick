namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class CurrentSkillProvider : ICurrentSkillProvider, ICurrentSkillSetter
    {
        private SkillID _currentSkill;

        public SkillID GetCurrentSkill()
        {
            return _currentSkill;
        }

        public void SetCurrentSkill(SkillID currencySkill)
        {
            _currentSkill = currencySkill;
        }
    }
}