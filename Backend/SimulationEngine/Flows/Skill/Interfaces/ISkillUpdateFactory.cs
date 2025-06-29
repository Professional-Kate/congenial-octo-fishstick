namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Skill skill, bool hasLeveled);
    }
}