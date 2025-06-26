namespace IdelPog.SimulationEngine.Flows.Skill
{
    /// <seealso cref="SwitchSkill"/>
    public interface ISkillController
    {
        /// <summary>
        /// Switches the currently active skill to the passed id <see cref="SkillID"/>
        /// </summary>
        /// <param name="skillChange">The <see cref="SkillID"/> you want to process a job completion on</param>
        public void SwitchSkill(SkillChange skillChange);
    }
}