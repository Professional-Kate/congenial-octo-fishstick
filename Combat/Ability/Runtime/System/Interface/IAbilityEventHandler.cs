using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEventHandler
    {
        public void Handle(ScheduledCombatEvent scheduledCombatEvent);
    }
}