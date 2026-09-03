using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Runtime.Event.Trigger.Interface
{
    public interface ITriggerReader
    {
        public ImmutableArray<AbilityEntity> GetAbilities(TriggerEventType triggerEventType);
    }
}