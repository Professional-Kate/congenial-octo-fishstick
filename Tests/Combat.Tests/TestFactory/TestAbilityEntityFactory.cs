using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityEntityFactory
    {
        internal static AbilityEntity Create(AbilityType abilityType, byte abilitySlots)
        {
            ElementalDamageComponent elementalDamageComponent = new()
            {
                LightningDamage = 0,
                ColdDamage = 0,
                FireDamage = 0
            };

            PhysicalDamageComponent physicalDamageComponent = new()
            {
                SlashDamage = 1,
                StrikeDamage = 0,
                ThrustDamage = 0
            };
            
            return new AbilityEntity(new CooldownComponent { Cooldown = 1 }, elementalDamageComponent, physicalDamageComponent)
            {
                AbilityType = abilityType,
                AbilitySlots = abilitySlots,
                Information = new Information { Name = abilityType.ToString(), Description = "" }
            };
        }
    }
}