using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.Filter
{
    public sealed class HighestAttackSelector : ICombatantSelector
    {
        private readonly ICollectionAssertion _collectionAssertion;

        public HighestAttackSelector(ICollectionAssertion collectionAssertion)
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
                if (combatantStatsComponent.Health == 0)
                {
                    continue;
                }

                if (combatantStatsComponent.Attack <= highestAttack)
                {
                    continue;
                }

                highestAttack = combatantStatsComponent.Attack;
                highestAttackEntity = combatantEntity;
            }
            
            return highestAttackEntity;
        }
    }
}