using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event;

namespace IdelPog.Combat.Service.Interface
{
    public interface IAbilityEventScheduler
    {
        /// <summary>
        /// Will enqueue a new <see cref="CombatEvent"/> on tick: <paramref name="currentTick"/>
        /// </summary>
        /// <param name="currentTick">The Tick you want the <see cref="CombatEvent"/> running on</param>
        /// <param name="initiatingCombatantID">The CombatantID this event belongs to</param>
        /// <param name="abilityType">The <see cref="AbilityType"/> used</param>
        public void ScheduleEvent(double currentTick, byte initiatingCombatantID, AbilityType abilityType);

        public void EnqueueAbilityEvent(double currentTick, byte initiatingCombatantID, AbilityType abilityType);
    }
}