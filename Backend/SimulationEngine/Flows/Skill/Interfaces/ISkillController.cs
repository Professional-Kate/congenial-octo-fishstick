using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
{
    /// <seealso cref="ChangeSkill"/>
    public interface ISkillController
    {
        /// <summary>
        /// Switches the currently active skill
        /// </summary>
        /// <param name="setSkill">This command will contain the <see cref="SkillID"/> you want to switch to</param>
        public void ChangeSkill(SetSkill setSkill);
    }
}