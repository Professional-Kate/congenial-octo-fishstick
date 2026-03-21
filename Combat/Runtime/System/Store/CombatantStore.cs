using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Store
{
    public sealed class CombatantStore : ICombatantStore
    {
        public LowestHealthCombatant LowestHealthCombatant { get; private set; }
        public HighestAttackCombatant HighestAttackCombatant { get; private set; }

        private readonly ICombatantSelector _lowestHealthSelector;
        private readonly ICombatantSelector _highestAttackSelector;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly INumberAssertion _numberAssertion;
        
        public CombatantStore(ICombatantSelector lowestHealthSelector, ICombatantSelector highestAttackSelector, ICollectionAssertion collectionAssertion, INumberAssertion numberAssertion)
        {
            _lowestHealthSelector = lowestHealthSelector;
            _highestAttackSelector = highestAttackSelector;
            _collectionAssertion = collectionAssertion;
            _numberAssertion = numberAssertion;

            // initial values. Setting uint's to opposite means first RegisterCombatantChange() will trigger an update
            LowestHealthCombatant = new LowestHealthCombatant { CombatantID = 0, Health = uint.MaxValue };
            HighestAttackCombatant = new HighestAttackCombatant { CombatantID = 0, Attack = uint.MinValue };
        }

        public void RegisterInitial(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity[] combatantEntities = combatants.ToArray();
            _collectionAssertion.AssertHasElements(combatantEntities);
            
            RegisterLowestHealthCombatant(combatantEntities);
            RegisterHighestAttackCombatant(combatantEntities);
        }

        public void RegisterCombatantChange(byte combatantID, StatCard statCard)
        {
            _numberAssertion.AssertNumberNotZero(statCard.Health, statCard.ToString());
            
            if (statCard.Health < LowestHealthCombatant.Health)
            {
                LowestHealthCombatant = ConstructLowestHealthCombatant(combatantID, statCard.Health);
            } 
            if (statCard.Attack > HighestAttackCombatant.Attack)
            {
                HighestAttackCombatant = ConstructHighestAttackCombatant(combatantID, statCard.Attack);
            }
        }

        public void RegisterCombatantDeath(byte combatantID, IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity[] combatantEntities = combatants.ToArray();
            _collectionAssertion.AssertHasElements(combatantEntities);
            
            if (combatantID == LowestHealthCombatant.CombatantID)
            { 
                RegisterLowestHealthCombatant(combatantEntities);
            }
            if (combatantID == HighestAttackCombatant.CombatantID)
            { 
                RegisterHighestAttackCombatant(combatantEntities);
            }
        }

        private void RegisterLowestHealthCombatant(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity combatantEntity = _lowestHealthSelector.GetEntity(combatants);
            CombatantStatsComponent statsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(statsComponent.StatCard.Health, statsComponent.StatCard.ToString());
            
            LowestHealthCombatant = ConstructLowestHealthCombatant(combatantEntity.CombatantID, statsComponent.StatCard.Health);
        }

        private void RegisterHighestAttackCombatant(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity combatantEntity = _highestAttackSelector.GetEntity(combatants);
            CombatantStatsComponent statsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(statsComponent.StatCard.Health, statsComponent.StatCard.ToString());
            
            HighestAttackCombatant = ConstructHighestAttackCombatant(combatantEntity.CombatantID, statsComponent.StatCard.Attack);
        }

        private static LowestHealthCombatant ConstructLowestHealthCombatant(byte combatantID, uint health) => new() { CombatantID = combatantID, Health = health };
        
        private static HighestAttackCombatant ConstructHighestAttackCombatant(byte combatantID, uint attack) => new() { CombatantID = combatantID, Attack = attack };
    }
}