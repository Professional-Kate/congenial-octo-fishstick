using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityDefinitionFactory
    {
        internal static AbilityDefinition Create(AbilityStageCard[] abilityStages, byte abilityID)
        {
            return new AbilityDefinition
            {
                AbilityStages = [..abilityStages],
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 1 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 }
            };
        }
        
        internal static AbilityDefinition Create(byte abilitySlots = 1)
        {
            return new AbilityDefinition
            {
                AbilityStages = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Priority = 0, Value = 1 }],
                AbilityCard = new AbilityCard { AbilitySlots = abilitySlots, Cooldown = 1 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 }
            };
        }
    }
}