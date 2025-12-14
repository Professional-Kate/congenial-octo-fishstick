using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Assertion
{
    public sealed class CombatantStatsAssertion : ICombatantStatsAssertion
    {
        private readonly IAmountAssertion _amountAssertion;

        public CombatantStatsAssertion(IAmountAssertion amountAssertion)
        {
            _amountAssertion = amountAssertion;
        }

        public void AssertStats(CombatantStats combatantStats)
        { 
            _amountAssertion.AssertAmountNotZero(combatantStats.Attack);
            _amountAssertion.AssertAmountNotZero(combatantStats.Health);
            _amountAssertion.AssertAmountNotZero(combatantStats.Speed);
        }
    }
}