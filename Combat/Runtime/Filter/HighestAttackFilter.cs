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

        public CombatantEntity GetEntity(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity[] combatantEntities = combatants.ToArray();
            _collectionAssertion.AssertHasElements(combatantEntities);
            
            uint highestAttack = uint.MinValue;
            CombatantEntity highestAttackEntity = combatantEntities.First();
            
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                if (combatantStatsComponent.StatCard.Health == 0)
                {
                    continue;
                }

                if (combatantStatsComponent.StatCard.Attack <= highestAttack)
                {
                    continue;
                }

                highestAttack = combatantStatsComponent.StatCard.Attack;
                highestAttackEntity = combatantEntity;
            }
            
            return highestAttackEntity;
        }
    }
}