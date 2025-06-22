using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    /// <seealso cref="SwitchSkill"/>
    public interface ISkillController
    {
        /// <summary>
        /// Switches the currently active skill to the passed id <see cref="SkillID"/>
        /// </summary>
        /// <param name="skillChange">The <see cref="SkillID"/> you want to process a job completion on</param>
        /// <returns>A <see cref="ServiceResponse"/> object on the state of the operation</returns>
        public ServiceResponse SwitchSkill(SkillChange skillChange);
    }
}