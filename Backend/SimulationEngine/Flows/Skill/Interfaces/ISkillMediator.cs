using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    /// <seealso cref="ProcessSkillAction"/>
    public interface ISkillMediator
    {
        /// <summary>
        /// Processes a <see cref="Skill"/> action. What each <see cref="Skill"/> does per action is defined by the <see cref="Skill"/> itself
        /// </summary>
        /// <param name="skillID">The <see cref="SkillID"/> you want to process an action on</param>
        /// <returns>A <see cref="ServiceResponse"/> which will tell you if the operation was successful</returns>
        /// <remarks>
        /// This method will only ever return a <see cref="ServiceResponse"/>, so, if anything goes wrong it'll be wrapped in this object.
        /// </remarks>
        public ServiceResponse ProcessSkillAction(SkillID skillID);
    }
}