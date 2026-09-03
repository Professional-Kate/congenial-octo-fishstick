using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEventScheduler
    {
        /// <summary>
        /// Will enqueue a new <see cref="ScheduledCombatEvent"/> on tick: <paramref name="forTick"/>
        /// </summary>
        /// <param name="forTick">The Tick you want the <see cref="ScheduledCombatEvent"/> running on</param>
        /// <param name="abilityID">The ability ID used</param>
        /// <param name="abilityStageIndex">The index of the ability stage</param>
        /// <param name="initiatingCombatantID">The CombatantID this event belongs to</param>
        public void ScheduleEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID);

        public void EnqueueAbilityExecuteEvent(double forTick, byte abilityID, byte abilityStageIndex, byte initiatingCombatantID);
    }
}