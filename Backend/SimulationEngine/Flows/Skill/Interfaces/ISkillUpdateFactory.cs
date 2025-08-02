namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillUpdateFactory
    {
        public SkillUpdateResponse CreateSkillUpdate(Models.Skill skill, bool hasLeveled);
    }
}