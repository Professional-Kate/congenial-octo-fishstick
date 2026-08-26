using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDamageSystem
    {
        /// <summary>
        /// Reduce the passed <see cref="CombatantEntity"/>s health, dealing damage to it
        /// </summary>
        /// <param name="targetCombatant">The <see cref="CombatantEntity"/> you want to deal damage to</param>
        /// <param name="combatantAbilityStage"><see cref="CombatantAbilityStage"/> is used to get the Abilities damage</param>
        /// <returns>The <paramref name="targetCombatant"/>s new Health</returns>
        public uint DealDamage(CombatantEntity targetCombatant, CombatantAbilityStage combatantAbilityStage);
        
        public uint GetCalculatedDamage(CombatantAbilityStage combatantAbilityStage);
    }
}