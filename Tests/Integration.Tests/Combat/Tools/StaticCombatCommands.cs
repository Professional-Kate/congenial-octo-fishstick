using IdelPog.Combat.Contracts.Ability;
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
            StatCard = new StatCard { Health = 25, Attack = 5 },
            AgilityCard = new AgilityCard { Speed = 7, Initiative = 2 },
            Information = new Information { Name = "John Idle", Description = "He the man" }
        };
        
        internal static readonly CombatantCreation GoblinCreation = new()
        {
            CombatantType = CombatantType.GOBLIN, 
            StatCard = new StatCard { Health = 9, Attack = 2 },
            AgilityCard = new AgilityCard { Speed = 11, Initiative = 3 },
            Information = new Information { Name = "Goblin", Description = "green guy" }
        };
        
        internal static readonly CombatantCreation BearCreation = new()
        {
            CombatantType = CombatantType.BEAR,
            StatCard = new StatCard { Health = 20, Attack = 15 },
            AgilityCard = new AgilityCard { Speed = 15, Initiative = 4 },
            Information = new Information { Name = "Bear", Description = "rawr" }
        };
        
        internal static readonly CombatantCreation WolfCreation = new()
        {
            CombatantType = CombatantType.WOLF,
            StatCard = new StatCard { Health = 3, Attack = 7 },
            AgilityCard = new AgilityCard { Speed = 17, Initiative = 1 },
            Information = new Information { Name = "Wolf", Description = "awoooo" }
        };
        
        internal static readonly AbilityCreation BasicAttackCreation = new()
        {
            AbilityType = AbilityType.BASIC_ATTACK, 
            EventType = EventType.DIRECT_DAMAGE,
            DamageCard = new DamageCard { PhysicalDamage = 1, ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
            Cooldown = 5,
            Information = new Information { Name = "Basic Attack!", Description = "Kinda weak.." },
            AbilitySlots = 1,
            CastTime = 0
        };
        
        internal static readonly AbilityCreation StrongAttackCreation = new()
        {
            AbilityType = AbilityType.STRONG_ATTACK, 
            EventType = EventType.DIRECT_DAMAGE,
            DamageCard = new DamageCard { PhysicalDamage = 5, ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
            Cooldown = 15,
            Information = new Information { Name = "Strong attack!", Description = "Wack them!!!" },
            AbilitySlots = 1,
            CastTime = 0
        };

        internal static CombatantAbilityEquip EquipBasicAttack(byte combatantID) => EquipAbility(combatantID, AbilityType.BASIC_ATTACK);

        internal static CombatantAbilityEquip EquipStrongAttack(byte combatantID) => EquipAbility(combatantID, AbilityType.STRONG_ATTACK);
        
        private static CombatantAbilityEquip EquipAbility(byte combatantID, AbilityType abilityType) => new()
        {
            CombatantID = combatantID, 
            AbilityCards = 
            [
                new AbilityCard
                {
                    AbilityType = abilityType, 
                    StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }
                }
            ]
        };
    }
}