using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class TestAbilityEntityFactory
    {
        internal static AbilityEntity Create(AbilityType abilityType, byte abilitySlots)
        {
            DamageComponent damageComponent = new()
            {
                PhysicalDamage = 1,
                LightningDamage = 0,
                ColdDamage = 0,
                FireDamage = 0
            };
            
            return new AbilityEntity(new CooldownComponent { Cooldown = 1 }, damageComponent)
            {
                AbilityType = abilityType,
                AbilitySlots = abilitySlots,
                Information = new Information { Name = abilityType.ToString(), Description = "" }
            };
        }
    }
}