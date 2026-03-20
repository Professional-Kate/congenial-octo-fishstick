using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.Filter
{
    public sealed class LowestHealthFilter : ICombatantFilter
    {
        private readonly ICollectionAssertion _collectionAssertion;

        public LowestHealthFilter(ICollectionAssertion collectionAssertion)
        {
            _collectionAssertion = collectionAssertion;
        }

        public byte GetEntity(CombatantEntity[] combatants)
        {
            _collectionAssertion.AssertHasElements(combatants);
            
            uint lowestHealth = uint.MaxValue;
            byte combatantID = 0;
            
            foreach (CombatantEntity combatantEntity in combatants)
            {
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                if (combatantStatsComponent.StatCard.Health >= lowestHealth)
                {
                    continue;
                }

                if (combatantStatsComponent.StatCard.Health == 1)
                {
                    return combatantEntity.CombatantID;
                }

                lowestHealth = combatantStatsComponent.StatCard.Health;
                combatantID = combatantEntity.CombatantID;
            }
            
            return combatantID;
        }
    }
}