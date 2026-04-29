using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal sealed class CombatantTracker
    {
        internal CombatantCreation CombatantCreation { get; set; }
        internal List<AbilityTracker> AbilityTrackers { get; } = [];
        internal int TotalAttacks { get; set; }
        
        internal CombatantTracker(int totalAttacks)
        {
            TotalAttacks = totalAttacks;
        }
        
        internal CombatantTracker(CombatantCreation combatantCreation)
        {
            CombatantCreation = combatantCreation;
        }

        internal void RegisterAbilityUse(AbilityType abilityType, uint damage)
        {
            foreach (AbilityTracker abilityTracker in AbilityTrackers.Where(abilityTracker => abilityTracker.AbilityType == abilityType))
            {
                abilityTracker.Attacks++;
                abilityTracker.TotalDamage += damage;
                return;
            }

            AbilityTrackers.Add(new AbilityTracker { AbilityType = abilityType, Attacks = 1, TotalDamage = damage });
        } 
    }
}