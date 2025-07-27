namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Models.Skill skill, bool hasLeveled);
    }
}