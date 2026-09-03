using System.Collections.Immutable;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service.Logging
{
    public sealed class CombatantLogger : ICombatantLogger
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly List<CombatantStateChange> _combatantStateChanges = [];
        private readonly List<AbilityStageLog> _combatantLog = [];

        public CombatantLogger(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        private AbilityStageLog _currentLog;

        public void LogCombatantChange(double tick, CombatantEntity initiatingCombatant, IReadOnlyList<CombatantEntity> targetCombatants,
            AbilityStageCard abilityStage, byte abilityID)
        {
            _objectNullAssertion.AssertNotNull(initiatingCombatant, nameof(initiatingCombatant));
            _collectionAssertion.AssertHasElements(targetCombatants);

            if (_combatantStateChanges.Count == 0)
            {
                _currentLog = CreateAbilityStageLog(initiatingCombatant, abilityID, tick);
            }

            ReadOnlyAbilityStage readOnlyAbilityStage = new()
            {
                AbilityEffectType = abilityStage.AbilityEffectType,
                AffinityType = abilityStage.AffinityType,
                Value = abilityStage.Value
            };
            
            CombatantStateChange combatantStateChange = new()
            {
                Tick = tick,
                TargetCombatants = CreateReadOnlyCombatants(targetCombatants),
                ReadOnlyAbilityStage = readOnlyAbilityStage
            };

            if (ShouldCreateNewLog(abilityID, initiatingCombatant.InstanceID, tick))
            {
                FinalizeCurrentLog();
                
                _currentLog = CreateAbilityStageLog(initiatingCombatant, abilityID, tick); 
            }

            _combatantStateChanges.Add(combatantStateChange);
        }

        public IReadOnlyList<CombatStage> GetStateChanges()
        {
            FinalizeCurrentLog();
            
            CombatStage[] finalCombatStages = new CombatStage[_combatantLog.Count];
            for (int i = 0; i < _combatantLog.Count; i++)
            {
                AbilityStageLog abilityStageLog = _combatantLog[i];
                finalCombatStages[i] = new CombatStage
                {
                    AbilityID = abilityStageLog.AbilityID,
                    InitiatingCombatant = abilityStageLog.InitiatingCombatant,
                    CombatantStateChanges = [..abilityStageLog.CombatantStateChanges]
                };
            }

            ClearStateChanges();
            return finalCombatStages;
        }

        public void ClearStateChanges()
        {
            _combatantStateChanges.Clear();
            _combatantLog.Clear();
        }

        private bool ShouldCreateNewLog(byte abilityID, byte combatantID, double tick)
        {
            bool isDifferentAbility = _currentLog.AbilityID != abilityID;
            bool isDifferentCombatant = _currentLog.InitiatingCombatant.InstanceID != combatantID;
            bool isDifferentExecutionTick = Math.Abs(_currentLog.Tick - tick) > 0.1;
            
            return isDifferentAbility || isDifferentCombatant || isDifferentExecutionTick;
        }

        private void FinalizeCurrentLog()
        {
            if (_combatantStateChanges.Count == 0)
            {
                return;
            }

            _currentLog.CombatantStateChanges = _combatantStateChanges.ToArray();
            _combatantLog.Add(_currentLog);
            _combatantStateChanges.Clear();
        }

        private static AbilityStageLog CreateAbilityStageLog(CombatantEntity combatantEntity, byte abilityID, double tick)
        {
            return new AbilityStageLog
            {
                Tick = tick,
                AbilityID = abilityID,
                InitiatingCombatant = CreateReadOnlyCombatant(combatantEntity),
                CombatantStateChanges = []
            };
        }
        
        private static ReadOnlyCombatant CreateReadOnlyCombatant(CombatantEntity combatantEntity)
        {
            return new ReadOnlyCombatant
            {
                InstanceID = combatantEntity.InstanceID,
                CombatantID = combatantEntity.CombatantID,
                StatCard = CreateStatCard(combatantEntity.GetComponent<HealthComponent>()),
                AgilityCard = CreateAgilityCard(combatantEntity.GetComponent<AgilityComponent>()),
                TargetingType = combatantEntity.TargetingType,
                IsAlive = combatantEntity.GetComponent<LifeStatusComponent>().IsAlive
            };
        }
        
        private static ImmutableArray<ReadOnlyCombatant> CreateReadOnlyCombatants(IReadOnlyList<CombatantEntity> combatantEntities)
        {
            ReadOnlyCombatant[] combatants = new ReadOnlyCombatant[combatantEntities.Count];
            for (int i = 0; i < combatantEntities.Count; i++)
            { 
                combatants[i] = CreateReadOnlyCombatant(combatantEntities[i]);
            }

            return [..combatants];
        }
        
        private static StatCard CreateStatCard(HealthComponent healthComponent)
        {
            return new StatCard
            {
                Health = healthComponent.Health
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
     }
    
    public record struct AbilityStageLog
    {
        public required double Tick { get; init; }
        public required byte AbilityID { get; init; }
        public required ReadOnlyCombatant InitiatingCombatant { get; init; }
        public required CombatantStateChange[] CombatantStateChanges { get; set; }
    }
}