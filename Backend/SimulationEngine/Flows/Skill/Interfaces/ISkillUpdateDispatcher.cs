namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillUpdateDispatcher
    {
        public void Dispatch(SkillUpdateDTO skillUpdateDTO);
    }
}