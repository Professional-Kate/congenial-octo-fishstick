using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal static class StaticCombatCommands
    {
        internal static readonly CombatantCreation HumanCreation = new()
        {
            CombatantType = CombatantType.HUMAN, 
            StatCard = new StatCard { Health = 25 },
            AgilityCard = new AgilityCard { Speed = 7, Initiative = 2 }
        };
        
        internal static readonly CombatantCreation GoblinCreation = new()
        {
            CombatantType = CombatantType.GOBLIN, 
            StatCard = new StatCard { Health = 9 },
            AgilityCard = new AgilityCard { Speed = 11, Initiative = 3 }
        };
        
        internal static readonly CombatantCreation BearCreation = new()
        {
            CombatantType = CombatantType.BEAR,
            StatCard = new StatCard { Health = 20 },
            AgilityCard = new AgilityCard { Speed = 15, Initiative = 4 }
        };
        
        internal static readonly CombatantCreation WolfCreation = new()
        {
            CombatantType = CombatantType.WOLF,
            StatCard = new StatCard { Health = 11 },
            AgilityCard = new AgilityCard { Speed = 17, Initiative = 1 }
        };

        internal static readonly TriggerCard AbilityReadyTrigger = new()
            { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 };
        
        internal static readonly AbilityCreation SlashAttackCreation = new()
        {
            AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
            TriggerCard = AbilityReadyTrigger,
            AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 1, Priority = 0 } ]
        };
        
        internal static readonly AbilityCreation StabAttackCreation = new()
        {
            AbilityCard = new AbilityCard { Cooldown = 15, AbilitySlots = 1 },
            TriggerCard = AbilityReadyTrigger,
            AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STAB, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 0 } ]
        };
        
        internal static readonly AbilityCreation StrikeAttackCreation = new()
        {
            AbilityCard = new AbilityCard { Cooldown = 10, AbilitySlots = 1 },
            TriggerCard = AbilityReadyTrigger,
            AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, CastTime = 0,  MaxTargets = 1, Value = 3, Priority = 0 } ]
        };

        /// <summary>
        /// This command assumes <see cref="SlashAttackCreation"/> was dispatched first
        /// </summary>
        internal static AbilityEquip EquipSlashAttack(byte combatantID, byte abilityID = 0) => EquipAbility(combatantID, abilityID);

        /// <summary>
        /// This command assumes <see cref="StabAttackCreation"/> was dispatched second
        /// </summary>
        internal static AbilityEquip EquipStabAttack(byte combatantID, byte abilityID = 1) => EquipAbility(combatantID, abilityID);
        
        /// <summary>
        /// This command assumes <see cref="StrikeAttackCreation"/> was dispatched third
        /// </summary>
        internal static AbilityEquip EquipStrikeAttack(byte combatantID, byte abilityID = 2) => EquipAbility(combatantID, abilityID);
        
        internal static AbilityEquip EquipAbilityCards(byte combatantID, params EquippedAbility[] abilityCards) => new()
        {
            CombatantID = combatantID, 
            EquippedAbilities = abilityCards
        };
        
        internal static AbilityEquip EquipAbility(byte combatantID, byte abilityID) => new()
        {
            CombatantID = combatantID, 
            EquippedAbilities = 
            [
                new EquippedAbility
                {
                    AbilityID = abilityID, 
                    StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]
                }
            ]
        };
    }
}