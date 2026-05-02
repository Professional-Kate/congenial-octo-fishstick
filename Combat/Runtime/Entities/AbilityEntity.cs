using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Contracts;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities
{
    public sealed record AbilityEntity : Entity
    {
        public required AbilityType AbilityType { get; init; }
        public required byte AbilitySlots { get; init; }
        public required Information Information { get; init; }
        
        public AbilityEntity(CooldownComponent cooldownComponent, DamageComponent damageComponent) 
            : base(cooldownComponent, damageComponent)
        {
        }
    }
}