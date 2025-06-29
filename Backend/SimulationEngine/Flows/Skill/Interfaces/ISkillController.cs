namespace IdelPog.SimulationEngine.Skill
{
    /// <seealso cref="SwitchSkill"/>
    public interface ISkillController
    {
        /// <summary>
        /// Switches the currently active skill
        /// </summary>
        /// <param name="skillChange">This command will contain the <see cref="SkillID"/> you want to switch to</param>
        public void SwitchSkill(SkillChange skillChange);
    }
}