using System.Collections.Immutable;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities
{
    public sealed record AbilityEntity : Entity
    {
        public required byte AbilitySlots { get; init; }
        public required ImmutableArray<AbilityStage> AbilityStages { get; init; }
        
        public AbilityEntity(CooldownComponent cooldownComponent, TriggerComponent triggerComponent) 
            : base(cooldownComponent, triggerComponent)
        {
        }
    }
}