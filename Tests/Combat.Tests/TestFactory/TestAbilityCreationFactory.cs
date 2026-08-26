using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityCreationFactory
    {
        public static AbilityCreation Create()
        {
            return Create(25);
        }

        private static AbilityCreation Create(uint cooldown)
        {
            return new AbilityCreation
            {
                AbilityCard = new AbilityCard {  Cooldown = cooldown, AbilitySlots = 1 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0, MaxTargets = 1, Value = 4, Priority = 1 }]
            };
        }
    }
}