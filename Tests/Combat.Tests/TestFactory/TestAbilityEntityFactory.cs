using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service;

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
            StatConfiguration statConfiguration = new()
            {
                CombatantStatLinks = new CombatantStatLinks
                {
                    BaseHealthID = 0,
                    HealthID = 1,
                    SpeedID = 2,
                    InitiativeID = 3
                },
                AbilityStatLinks = new AbilityStatLinks
                {
                    AbilityDamageID = 4,
                    AbilityHealingID = 5,
                    RetaliationDamageID = 6,
                    CastTimeID = 7,
                    CooldownID = 8,
                    AbilitySlotsID = 9
                }
            };
            
            StatConfigurationSystem statConfigurationSystem = new();
            statConfigurationSystem.SetStatConfiguration(statConfiguration);

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
                    new StatComponent { StatID = 4, Value = 0 },
                    new StatComponent { StatID = 5, Value = 0 },
                    new StatComponent { StatID = 6, Value = 0 },
                    new StatComponent { StatID = 7, Value = 0 },
                    new StatComponent { StatID = 8, Value = 0 },
                    new StatComponent { StatID = 9, Value = 0 }
                ]
            };
            return new AbilityEntity(statConfigurationSystem, statsComponent, triggerComponent, new AbilityStagesComponent { AbilityStages = [..combatantAbilityStages] })
            {
                InstanceID = instanceID, 
                AbilityID = abilityID
            };
        }

    }
}