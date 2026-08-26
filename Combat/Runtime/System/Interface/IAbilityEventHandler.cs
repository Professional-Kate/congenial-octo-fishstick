using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IAbilityEventHandler
    {
        public void Handle(ScheduledCombatEvent scheduledCombatEvent);
    }
}