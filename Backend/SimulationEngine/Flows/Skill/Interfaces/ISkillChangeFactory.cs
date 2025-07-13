namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillChangeFactory
    {
        public SkillChangeDTO CreateSkillChangeDTO(SkillChange skillChange);
    }
}