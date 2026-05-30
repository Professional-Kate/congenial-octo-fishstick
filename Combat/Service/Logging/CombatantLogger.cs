using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
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

        public void LogCombatantChange(CombatantEntity changedEntity, byte attackerID, AbilityType abilityType, uint damageDealt, double tick)
        {
            _objectNullAssertion.AssertNotNull(changedEntity, nameof(changedEntity));
            
            CombatantStateChange combatantStateChange = new()
            {
                Tick = tick,
                CombatantCreation = CreateCombatantCard(changedEntity),
                CombatantID = changedEntity.CombatantID,
                IsFriendly = changedEntity.GetComponent<FriendlyStatusComponent>().IsFriendly,
                IsAlive = changedEntity.GetComponent<LifeStatusComponent>().IsAlive,
                AttackingCombatant =  CreateAttackingCombatant(attackerID, abilityType, damageDealt)
            };
            
            _combatantStateChanges.Add(combatantStateChange);
        }
        
        public IReadOnlyList<CombatantStateChange> GetStateChanges() => _combatantStateChanges;
        
        public void ClearStateChanges() => _combatantStateChanges.Clear();

        private static CombatantCreation CreateCombatantCard(CombatantEntity combatantEntity)
        {
            return new CombatantCreation
            {
                CombatantType = combatantEntity.CombatantType,
                StatCard = CreateStatCard(combatantEntity.GetComponent<StatsComponent>()),
                AgilityCard = CreateAgilityCard(combatantEntity.GetComponent<AgilityComponent>()),
                Information = combatantEntity.CombatantInformation
            };
        }

        private static StatCard CreateStatCard(StatsComponent statsComponent)
        {
            return new StatCard
            {
                Health = statsComponent.Health
            };
        }

        private static AgilityCard CreateAgilityCard(AgilityComponent agilityComponent)
        {
            return new AgilityCard
            {
                Speed = agilityComponent.Speed,
                Initiative = agilityComponent.Initiative
            };
        }

        private static AttackingCombatant CreateAttackingCombatant(byte attackerID, AbilityType abilityType, uint damageDealt)
        {
            return new AttackingCombatant
            {
                CombatantID = attackerID,
                AbilityType = abilityType,
                DamageDealt = damageDealt
            };
        }
     }
}