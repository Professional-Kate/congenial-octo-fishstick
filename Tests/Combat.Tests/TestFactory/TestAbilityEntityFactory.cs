using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestAbilityEntityFactory
    {
        internal static AbilityEntity Create(byte instanceID, byte abilityID)
        {
            AbilityStage abilityStage = new()
            {
                AbilityStageCards = new AbilityStageCard
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
                    CombatantStatType = CombatantStatType.HEALTH,
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
                AbilityStageCards = new AbilityStageCard
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
                    CombatantStatType = CombatantStatType.HEALTH,
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
            
            return new AbilityEntity(new CooldownComponent { Cooldown = 3 }, triggerComponent, new AbilityStagesComponent { AbilityStages = [..combatantAbilityStages] })
            {
                InstanceID = instanceID, 
                AbilityID = abilityID,
                AbilitySlots = 1
            };
        }

    }
}