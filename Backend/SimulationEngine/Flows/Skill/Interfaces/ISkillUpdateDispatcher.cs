namespace IdelPog.SimulationEngine.Flows.Skill
{
    public interface ISkillUpdateDispatcher
    {
        public void Dispatch(SkillUpdateDTO skillUpdateDTO);
    }
}