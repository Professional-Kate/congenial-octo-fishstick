namespace IdelPog.SimulationEngine.Skill
{
    /// <seealso cref="ProcessSkillAction"/>
    public interface ISkillActionMediator
    {
        /// <summary>
        /// Processes a <see cref="Skill"/> action. What each <see cref="Skill"/> does per action is defined by the <see cref="Skill"/> itself
        /// </summary>
        public void ProcessSkillAction();
    }
}