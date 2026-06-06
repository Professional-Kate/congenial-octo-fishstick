using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal static class StaticCombatCommands
    {
        internal static readonly CombatantCreation HumanCreation = new()
        {
            CombatantType = CombatantType.HUMAN, 
            StatCard = new StatCard { Health = 25 },
            AgilityCard = new AgilityCard { Speed = 7, Initiative = 2 },
            Information = new Information { Name = "John Idle", Description = "He the man" }
        };
        
        internal static readonly CombatantCreation GoblinCreation = new()
        {
            CombatantType = CombatantType.GOBLIN, 
            StatCard = new StatCard { Health = 9 },
            AgilityCard = new AgilityCard { Speed = 11, Initiative = 3 },
            Information = new Information { Name = "Goblin", Description = "green guy" }
        };
        
        internal static readonly CombatantCreation BearCreation = new()
        {
            CombatantType = CombatantType.BEAR,
            StatCard = new StatCard { Health = 20 },
            AgilityCard = new AgilityCard { Speed = 15, Initiative = 4 },
            Information = new Information { Name = "Bear", Description = "rawr" }
        };
        
        internal static readonly CombatantCreation WolfCreation = new()
        {
            CombatantType = CombatantType.WOLF,
            StatCard = new StatCard { Health = 3 },
            AgilityCard = new AgilityCard { Speed = 17, Initiative = 1 },
            Information = new Information { Name = "Wolf", Description = "awoooo" }
        };
        
        internal static readonly AbilityCreation SlashAttackCreation = new()
        {
            AbilityCard = new AbilityCard {  AbilityType = AbilityType.SLASH, EventType = EventType.DIRECT_DAMAGE, Cooldown = 5, AbilitySlots = 1, CastTime = 0},
            ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
            PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 1, StrikeDamage = 0, ThrustDamage = 0 },
            Information = new Information { Name = "Slash!", Description = "Slashing!" }
        };
        
        internal static readonly AbilityCreation StabAttackCreation = new()
        {
            AbilityCard = new AbilityCard {  AbilityType = AbilityType.STAB, EventType = EventType.DIRECT_DAMAGE, Cooldown = 15, AbilitySlots = 1, CastTime = 0},
            ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
            PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 5 },
            Information = new Information { Name = "Stab!", Description = "Thrust moment" }
        };
        
        internal static readonly AbilityCreation StrikeAttackCreation = new()
        {
            AbilityCard = new AbilityCard {  AbilityType = AbilityType.STRIKE, EventType = EventType.DIRECT_DAMAGE, Cooldown = 10, AbilitySlots = 1, CastTime = 0},
            ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
            PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 3, ThrustDamage = 0 },
            Information = new Information { Name = "Strike!", Description = "owie that hurt" }
        };

        internal static CombatantAbilityEquip EquipSlashAttack(byte combatantID) => EquipAbility(combatantID, AbilityType.SLASH);

        internal static CombatantAbilityEquip EquipStabAttack(byte combatantID) => EquipAbility(combatantID, AbilityType.STAB);
        
        internal static CombatantAbilityEquip EquipStrikeAttack(byte combatantID) => EquipAbility(combatantID, AbilityType.STRIKE);
        
        internal static CombatantAbilityEquip EquipAbilityCards(byte combatantID, params CombatantAbilityCard[] abilityCards) => new()
        {
            CombatantID = combatantID, 
            AbilityCards = abilityCards
        };
        
        internal static CombatantAbilityEquip EquipAbility(byte combatantID, AbilityType abilityType) => new()
        {
            CombatantID = combatantID, 
            AbilityCards = 
            [
                new CombatantAbilityCard
                {
                    AbilityType = abilityType, 
                    StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }
                }
            ]
        };
    }
}