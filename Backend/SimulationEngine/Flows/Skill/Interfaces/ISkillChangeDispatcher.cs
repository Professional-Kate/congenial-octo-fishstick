namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillChangeDispatcher
    {
        public void Dispatch(SkillChangeDTO skillChangeDTO);
    }
}