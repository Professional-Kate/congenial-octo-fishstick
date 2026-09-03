using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Event.Trigger.Interface
{
    public interface ITriggerReader
    {
        public ImmutableArray<AbilityEntity> GetAbilities(TriggerEventType triggerEventType);
    }
}