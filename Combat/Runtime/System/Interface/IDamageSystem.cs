using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDamageSystem
    {
        /// <summary>
        /// Reduce the passed <see cref="CombatantEntity"/>s health, dealing damage to it
        /// </summary>
        /// <param name="targetCombatant">The <see cref="CombatantEntity"/> you want to deal damage to</param>
        /// <param name="attackerAttack">Will be used to deal damage to the <paramref name="targetCombatant"/></param>
        /// <param name="attackerAbility"><see cref="CombatantAbilityEntity"/> is used to get the Abilities damage</param>
        /// <returns>The <paramref name="targetCombatant"/>s new Health</returns>
        public uint DealDamage(CombatantEntity targetCombatant, uint attackerAttack, CombatantAbilityEntity attackerAbility);
        
        public uint GetCalculatedDamage(uint attackerAttack, CombatantAbilityEntity attackerAbility);
    }
}