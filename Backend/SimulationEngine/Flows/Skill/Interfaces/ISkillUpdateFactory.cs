namespace IdelPog.SimulationEngine.Flows.Skill
{
    public interface ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Skill skill, bool canSkillLevel);
    }
}