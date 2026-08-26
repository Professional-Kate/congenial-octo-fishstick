using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityEntityFactory
    {
        internal static AbilityEntity Create(AbilityStage[] abilityStages)
        {
            return new AbilityEntity(new CooldownComponent { Cooldown = 1 }, new TriggerComponent { TargetingType = TargetingType.SELF, TriggerEventType = TriggerEventType.ABILITY_READY, MinTriggerValue = 0, MaxTriggerValue = 0 })
            {
                AbilitySlots = 1,
                AbilityStages = [..abilityStages]
            };
        }
        
        internal static AbilityEntity Create(byte abilitySlots = 1)
        {
            return new AbilityEntity(new CooldownComponent { Cooldown = 1 }, new TriggerComponent { TargetingType = TargetingType.SELF, TriggerEventType = TriggerEventType.ABILITY_READY, MinTriggerValue = 0, MaxTriggerValue = 0 })
            {
                AbilitySlots = abilitySlots,
                AbilityStages = [ new AbilityStage { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0, MaxTargets = 1, Value = 3, Priority = 0 }]
            };
        }
    }
}