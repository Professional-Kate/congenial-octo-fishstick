using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
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
                StatsComponent statsComponent = combatantEntity.GetComponent<StatsComponent>();
                if (statsComponent.Health == 0)
                {
                    continue;
                }
                
                if (statsComponent.Health >= lowestHealth)
                {
                    continue;
                }

                if (statsComponent.Health == 1)
                {
                    return combatantEntity;
                }

                lowestHealth = statsComponent.Health;
                highestAttackEntity =  combatantEntity;
            }
            
            return highestAttackEntity;
        }
    }
}