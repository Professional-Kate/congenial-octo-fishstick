using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDamageSystem
    {
        /// <summary>
        /// Reduce the passed <see cref="CombatantEntity"/>s health, dealing damage to it
        /// </summary>
        /// <param name="targetCombatant">The <see cref="CombatantEntity"/> you want to deal damage to</param>
        /// <param name="attackerStats">Will be used to deal damage to the <paramref name="targetCombatant"/></param>
        /// <returns>The <paramref name="targetCombatant"/>s new Health</returns>
        public uint DealDamage(CombatantEntity targetCombatant, StatCard attackerStats);
    }
}