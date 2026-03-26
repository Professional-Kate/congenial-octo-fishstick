using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.Filter
{
    public sealed class LowestHealthSelector : ICombatantSelector
    {
        private readonly ICollectionAssertion _collectionAssertion;

        public LowestHealthSelector(ICollectionAssertion collectionAssertion)
        {
            _collectionAssertion = collectionAssertion;
        }

        public CombatantEntity GetEntity(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity[] combatantEntities = combatants.ToArray();
            _collectionAssertion.AssertHasElements(combatantEntities);
            
            uint lowestHealth = uint.MaxValue;
            CombatantEntity highestAttackEntity = combatantEntities.First();
            
            
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                if (combatantStatsComponent.Health == 0)
                {
                    continue;
                }
                
                if (combatantStatsComponent.Health >= lowestHealth)
                {
                    continue;
                }

                if (combatantStatsComponent.Health == 1)
                {
                    return combatantEntity;
                }

                lowestHealth = combatantStatsComponent.Health;
                highestAttackEntity =  combatantEntity;
            }
            
            return highestAttackEntity;
        }
    }
}