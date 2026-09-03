using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
{
    public interface IDamageCalculator
    {
        /// <summary>
        /// Reduce the passed <see cref="CombatantEntity"/>s health, dealing damage to it
        /// </summary>
        /// <param name="targetCombatant">The <see cref="CombatantEntity"/> you want to deal damage to</param>
        /// <param name="abilityStage"><see cref="AbilityStage"/> is used to get the Abilities damage</param>
        /// <returns>The <paramref name="targetCombatant"/>s new Health</returns>
        public uint DealDamage(CombatantEntity targetCombatant, AbilityStage abilityStage);
        
        public uint GetCalculatedDamage(AbilityStage abilityStage);
    }
}