using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityCreationFactory
    {
        public static AbilityCreation Create(AbilityType abilityType)
        {
            return Create(abilityType, 25, 50);
        }

        private static AbilityCreation Create(AbilityType abilityType, uint cooldown, uint damage)
        {
            return new AbilityCreation
            {
                Information = new Information { Name = "", Description = "" },
                AbilityCard = new AbilityCard {  AbilityType = abilityType, EventType = EventType.DIRECT_DAMAGE, Cooldown = cooldown, AbilitySlots = 1, CastTime = 0},
                ElementalDamageCard = new ElementalDamageCard { LightningDamage = 0, FireDamage = 0, ColdDamage = 0 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = damage, StrikeDamage = 0, ThrustDamage = 0 },
            };
        }
    }
}