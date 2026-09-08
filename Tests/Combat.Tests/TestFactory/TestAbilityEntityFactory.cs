using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityEntityFactory
    {
        internal static AbilityEntity Create(byte instanceID, byte abilityID)
        {
            AbilityStage abilityStage = new()
            {
                AbilityStageCard = new AbilityStageCard
                {
                    AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, 
                    AffinityType = AffinityType.SLASH, 
                    MaxTargets = 1, 
                    Value = 3,
                    Priority = 0,
                    CastTime = 0
                },
                TargetingPreferenceComponent = new TargetingPreferenceComponent
                {
                    StatType = StatType.HEALTH,
                    TargetingPreference = TargetingPreference.HIGHEST,
                    TargetingType = TargetingType.ENEMY
                }
            };

            return Create(instanceID, abilityID, abilityStage);
        }
        
        internal static AbilityEntity CreateWithCastTime(byte instanceID, byte abilityID, uint castTime)
        {
            AbilityStage abilityStage = new()
            {
                AbilityStageCard = new AbilityStageCard
                {
                    AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, 
                    AffinityType = AffinityType.SLASH, 
                    MaxTargets = 1, 
                    Value = 3,
                    Priority = 0,
                    CastTime = castTime
                },
                TargetingPreferenceComponent = new TargetingPreferenceComponent
                {
                    StatType = StatType.HEALTH,
                    TargetingPreference = TargetingPreference.HIGHEST,
                    TargetingType = TargetingType.ENEMY
                }
            };
            
            AbilityEntity abilityEntity = Create(instanceID, abilityID, abilityStage);
            
            return abilityEntity;
        }
        
        internal static AbilityEntity Create(byte instanceID, byte abilityID, params AbilityStage[] combatantAbilityStages)
        {
            TriggerComponent triggerComponent = new()
            {
                TargetingType = TargetingType.SELF,
                TriggerEventType = TriggerEventType.ABILITY_READY,
                MinTriggerValue = 0,
                MaxTriggerValue = 0
            };

            StatsComponent statsComponent = new()
            {
                StatComponents =
                [
                    new StatComponent { StatType = StatType.ABILITY_SLOTS, Stat = 1 },
                    new StatComponent { StatType = StatType.COOLDOWN, Stat = 1 },
                    new StatComponent { StatType = StatType.CAST_TIME, Stat = 1 },
                    new StatComponent { StatType = StatType.ABILITY_DAMAGE, Stat = 1 },
                    new StatComponent { StatType = StatType.RETALIATION_DAMAGE, Stat = 1 },
                    new StatComponent { StatType = StatType.ABILITY_HEALING, Stat = 1 }
                ]
            };
            return new AbilityEntity(statsComponent, triggerComponent, new AbilityStagesComponent { AbilityStages = [..combatantAbilityStages] })
            {
                InstanceID = instanceID, 
                AbilityID = abilityID,
            };
        }

    }
}