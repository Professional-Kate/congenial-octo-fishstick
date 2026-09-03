using IdelPog.Combat.Ability.Runtime.Component;

namespace IdelPog.Combat.Ability.Runtime.Entity
{
    public sealed record AbilityEntity : ECS.Entity.Entity
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