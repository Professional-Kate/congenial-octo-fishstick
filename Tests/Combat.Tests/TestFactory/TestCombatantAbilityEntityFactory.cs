using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantAbilityEntityFactory
    {
        internal static CombatantAbilityEntity Create(byte combatantID, byte abilityID)
        {
            CombatantAbilityStage combatantAbilityStage = new()
            {
                AbilityStage = new AbilityStage
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

            return Create(combatantID, abilityID, combatantAbilityStage);
        }
        
        internal static CombatantAbilityEntity CreateWithCastTime(byte combatantID, byte abilityID, uint castTime)
        {
            CombatantAbilityStage combatantAbilityStage = new()
            {
                AbilityStage = new AbilityStage
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
            
            CombatantAbilityEntity combatantAbilityEntity = Create(combatantID, abilityID, combatantAbilityStage);
            
            return combatantAbilityEntity;
        }
        
        internal static CombatantAbilityEntity Create(byte combatantID, byte abilityID, params CombatantAbilityStage[] combatantAbilityStages)
        {
            TriggerComponent triggerComponent = new()
            {
                TargetingType = TargetingType.SELF,
                TriggerEventType = TriggerEventType.ABILITY_READY,
                MinTriggerValue = 0,
                MaxTriggerValue = 0
            };
            
            return new CombatantAbilityEntity(new CooldownComponent { Cooldown = 3 }, triggerComponent, new AbilityStagesComponent { AbilityStages = [..combatantAbilityStages] })
            {
                CombatantID = combatantID, 
                AbilityID = abilityID,
                AbilitySlots = 1
            };
        }

    }
}