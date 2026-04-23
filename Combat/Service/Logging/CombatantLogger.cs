using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service.Logging
{
    public sealed class CombatantLogger : ICombatantLogger
    {
        private readonly List<CombatantStateChange> _combatantStateChanges = [];
        private readonly IObjectNullAssertion _objectNullAssertion;

        public CombatantLogger(IObjectNullAssertion objectNullAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
        }

        public void LogCombatantChange(CombatantEntity changedEntity, byte attackerID)
        {
            _objectNullAssertion.AssertNotNull(changedEntity, nameof(changedEntity));
            
            CombatantStateChange combatantStateChange = new()
            {
                CombatantCard = CreateCombatantCard(changedEntity),
                CombatantID = changedEntity.CombatantID,
                AttackerID =  attackerID,
                IsFriendly = changedEntity.IsFriendly,
                IsAlive = changedEntity.GetComponent<LifeStatusComponent>().IsAlive
            };
            
            _combatantStateChanges.Add(combatantStateChange);
        }
        
        public IReadOnlyList<CombatantStateChange> GetStateChanges() => _combatantStateChanges;
        
        public void ClearStateChanges() => _combatantStateChanges.Clear();

        private static CombatantCard CreateCombatantCard(CombatantEntity combatantEntity)
        {
            return new CombatantCard
            {
                CombatantType = combatantEntity.CombatantType,
                TargetingType = combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType,
                StatCard = CreateStatCard(combatantEntity.GetComponent<CombatantStatsComponent>()),
                Information = combatantEntity.CombatantInformation
            };
        }

        private static StatCard CreateStatCard(CombatantStatsComponent combatantStatsComponent)
        {
            return new StatCard
            {
                Attack = combatantStatsComponent.Attack,
                Health = combatantStatsComponent.Health,
                Speed = combatantStatsComponent.Speed
            };
        }
     }
}