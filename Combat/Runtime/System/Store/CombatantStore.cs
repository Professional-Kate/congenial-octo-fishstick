using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
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

            // initial values. Setting uint's to opposite means first RegisterCombatantChange() will trigger an update even without RegisterInitial()
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

        public void RegisterCombatantChange(byte combatantID, CombatantStatsComponent combatantStatsComponent)
        {
            _numberAssertion.AssertNumberNotZero(combatantStatsComponent.Health, combatantStatsComponent.ToString());
            
            if (combatantStatsComponent.Health < LowestHealthCombatant.Health)
            {
                LowestHealthCombatant = ConstructLowestHealthCombatant(combatantID, combatantStatsComponent.Health);
            } 
            if (combatantStatsComponent.Attack > HighestAttackCombatant.Attack)
            {
                HighestAttackCombatant = ConstructHighestAttackCombatant(combatantID, combatantStatsComponent.Attack);
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
            _numberAssertion.AssertNumberNotZero(statsComponent.Health, statsComponent.ToString());
            
            LowestHealthCombatant = ConstructLowestHealthCombatant(combatantEntity.CombatantID, statsComponent.Health);
        }

        private void RegisterHighestAttackCombatant(IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity combatantEntity = _highestAttackSelector.GetEntity(combatants);
            CombatantStatsComponent statsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(statsComponent.Health, statsComponent.ToString());
            
            HighestAttackCombatant = ConstructHighestAttackCombatant(combatantEntity.CombatantID, statsComponent.Attack);
        }

        private static LowestHealthCombatant ConstructLowestHealthCombatant(byte combatantID, uint health) => new() { CombatantID = combatantID, Health = health };
        
        private static HighestAttackCombatant ConstructHighestAttackCombatant(byte combatantID, uint attack) => new() { CombatantID = combatantID, Attack = attack };
    }
}