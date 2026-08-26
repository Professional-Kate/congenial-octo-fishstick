using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities.Combatant
{
    public sealed record CombatantAbilityEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required byte AbilityID { get; init; }
        public required byte AbilitySlots { get; init; }

        public CombatantAbilityEntity(CooldownComponent cooldownComponent, TriggerComponent triggerComponent, AbilityStagesComponent abilityStagesComponent) 
            : base(cooldownComponent, triggerComponent, abilityStagesComponent)
        {
        }
    }
}