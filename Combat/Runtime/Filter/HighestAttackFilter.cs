using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.Filter
{
    public sealed class HighestAttackFilter : ICombatantFilter
    {
        private readonly ICollectionAssertion _collectionAssertion;

        public HighestAttackFilter(ICollectionAssertion collectionAssertion)
        {
            _collectionAssertion = collectionAssertion;
        }

        public byte GetEntity(CombatantEntity[] combatants)
        {
            _collectionAssertion.AssertHasElements(combatants);
            
            uint highestAttack = uint.MinValue;
            byte combatantID = 0;
            
            foreach (CombatantEntity combatantEntity in combatants)
            {
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                if (combatantStatsComponent.StatCard.Attack <= highestAttack)
                {
                    continue;
                }

                highestAttack = combatantStatsComponent.StatCard.Attack;
                combatantID = combatantEntity.CombatantID;
            }
            
            return combatantID;
        }
    }
}