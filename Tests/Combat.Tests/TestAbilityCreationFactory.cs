using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class TestAbilityCreationFactory
    {
        public static AbilityCreation Create(AbilityType abilityType)
        {
            return Create(abilityType, 25, 50);
        }
        
        public static AbilityCreation Create(AbilityType abilityType, uint speed, uint damage)
        {
            return new AbilityCreation
            {
                Information = new Information { Name = "", Description = "" },
                AbilityType = abilityType,
                EventType = EventType.DIRECT_DAMAGE,
                Cooldown = speed,
                DamageCard = new DamageCard { PhysicalDamage = damage,  LightningDamage = 0, FireDamage = 0, ColdDamage = 0 },
                AbilitySlots = 1,
                CastTime = 0
            };
        }
    }
}