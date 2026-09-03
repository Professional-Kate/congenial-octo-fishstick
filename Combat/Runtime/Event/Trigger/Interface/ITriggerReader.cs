using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.Event.Trigger.Interface
{
    public interface ITriggerReader
    {
        public ImmutableArray<AbilityEntity> GetAbilities(TriggerEventType triggerEventType);
    }
}