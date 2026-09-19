using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEventScheduler
    {
        /// <summary>
        /// Will enqueue a new <see cref="ScheduledCombatEvent"/> on tick: <paramref name="forTick"/>
        /// </summary>
        /// <param name="forTick">The Tick you want the <see cref="ScheduledCombatEvent"/> running on</param>
        /// <param name="abilityEntity">The AbilityEntity used</param>
        /// <param name="abilityStageIndex">The index of the ability stage</param>
        /// <param name="combatantEntity">The CombatantEntity this event belongs too</param>
        public void ScheduleEvent(double forTick, AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity);

        public void EnqueueAbilityExecuteEvent(double forTick, AbilityEntity abilityEntity, byte abilityStageIndex, CombatantEntity combatantEntity);
    }
}