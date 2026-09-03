using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Ability.Runtime.Entities
{
    public sealed record AbilityEntity : Entity
    {
        public required byte InstanceID { get; init; }
        public required byte AbilityID { get; init; }
        public required byte AbilitySlots { get; init; }

        public AbilityEntity(CooldownComponent cooldownComponent, TriggerComponent triggerComponent, AbilityStagesComponent abilityStagesComponent) 
            : base(cooldownComponent, triggerComponent, abilityStagesComponent)
        {
        }
    }
}