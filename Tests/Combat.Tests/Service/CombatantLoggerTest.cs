using System.Collections.Immutable;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Logging;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatantLoggerTest
    {
        private CombatantLogger _combatantLogger;

        private const double TICK = 100d;
        private const byte ABILITY_ID = 1;
        private CombatantEntity _initiatingCombatant;
        private CombatantEntity _targetCombatant;
        private AbilityStageCard _directDamageStage;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantLogger = new CombatantLogger(new ObjectNullAssertion(), new CollectionAssertion());

            _directDamageStage = new AbilityStageCard
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE,
                AffinityType = AffinityType.LIGHTNING,
                MaxTargets = 1,
                Value = 3,
                Priority = 0,
                CastTime = 0
            };
        }

        [SetUp]
        public void Setup()
        {
            _initiatingCombatant = TestCombatantEntityFactory.Create(combatantID: 1, TargetingType.FRIENDLY);
            _targetCombatant = TestCombatantEntityFactory.Create(combatantID: 2, TargetingType.ENEMY);
        }
        
        [TearDown]
        public void TearDown()
        { 
            _combatantLogger.ClearStateChanges();
        }

        private static void AssertCombatStageLength(IReadOnlyList<CombatStage> stateChanges, int expectedLength)
        {
            Assert.That(stateChanges, Has.Count.EqualTo(expectedLength));
        }

        private static void AssertStateChangeLength(ImmutableArray<CombatantStateChange> combatantStateChanges, int expectedLength)
        {
            Assert.That(combatantStateChanges, Has.Length.EqualTo(expectedLength));
        }

        private static void AssertStateChange(CombatStage combatStage, CombatantStateChange combatantStateChange, CombatantEntity initiatingCombatant, CombatantEntity[] targetCombatants, AbilityStageCard abilityStage)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatStage.AbilityID, Is.EqualTo(ABILITY_ID));
                Assert.That(combatStage.InitiatingCombatant.InstanceID, Is.EqualTo(initiatingCombatant.InstanceID));
                Assert.That(combatStage.InitiatingCombatant.TargetingType, Is.EqualTo(initiatingCombatant.TargetingType));
                Assert.That(combatantStateChange.ReadOnlyAbilityStage.AbilityEffectType, Is.EqualTo(abilityStage.AbilityEffectType));
                Assert.That(combatantStateChange.ReadOnlyAbilityStage.AffinityType, Is.EqualTo(abilityStage.AffinityType));
                Assert.That(combatantStateChange.ReadOnlyAbilityStage.Value, Is.EqualTo(abilityStage.Value));

                for (int i = 0; i < combatantStateChange.TargetCombatants.Length; i++)
                {
                    ReadOnlyCombatant readOnlyCombatant = combatantStateChange.TargetCombatants[i];
                    CombatantEntity combatantEntity = targetCombatants[i];
                    
                    Assert.That(readOnlyCombatant.InstanceID, Is.EqualTo(combatantEntity.InstanceID));
                    Assert.That(readOnlyCombatant.TargetingType,Is.EqualTo(combatantEntity.TargetingType));
                }
            }
        }

        [Test]
        public void Positive_LogCombatantChange_LogsEntity()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));

            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 1);
            AssertStateChangeLength(stateChanges[0].CombatantStateChanges, 1);
            AssertStateChange(stateChanges[0], stateChanges[0].CombatantStateChanges[0], _initiatingCombatant, [_targetCombatant], _directDamageStage);
        }
        
        [Test]
        public void Positive_LogCombatantChange_MultipleLogs_DifferentCombatStage()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _targetCombatant, [_initiatingCombatant], _directDamageStage, ABILITY_ID));

            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 2);
            AssertStateChange(stateChanges[0], stateChanges[0].CombatantStateChanges[0], _initiatingCombatant, [_targetCombatant], _directDamageStage);
            AssertStateChange(stateChanges[1], stateChanges[1].CombatantStateChanges[0], _targetCombatant, [_initiatingCombatant], _directDamageStage);
        }

        [Test]
        public void Positive_LogCombatantChange_MultipleLogs_SameCombatStage()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage with { AbilityEffectType = AbilityEffectType.HEALING }, ABILITY_ID));
            
            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 1);
            AssertStateChange(stateChanges[0], stateChanges[0].CombatantStateChanges[0], _initiatingCombatant, [_targetCombatant], _directDamageStage);
            AssertStateChange(stateChanges[0], stateChanges[0].CombatantStateChanges[1], _initiatingCombatant, [_targetCombatant], _directDamageStage with { AbilityEffectType = AbilityEffectType.HEALING });
        }

        [Test]
        public void Positive_LogCombatantChange_ThreeDifferentLogs()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID + 1));
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK + 1, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            
            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 3);
        }

        [Test]
        public void Positive_GetStateChanges_NoStateChanges_ReturnsEmptyArray()
        {
            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 0);
        }

        [Test]
        public void Positive_GetStageChanges_ClearsStateChanges()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            
            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 1);
            Assert.That(_combatantLogger.GetStateChanges(), Is.Empty);
        }

        [Test]
        public void Positive_ClearStateChanges_ClearsStates()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [_targetCombatant], _directDamageStage, ABILITY_ID));
            AssertCombatStageLength(_combatantLogger.GetStateChanges(), 1);
            
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            AssertCombatStageLength( _combatantLogger.GetStateChanges(), 0);
        }

        [Test]
        public void Positive_ClearStateChanges_NoChanges_DoesNothing()
        {
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            
            IReadOnlyList<CombatStage> stateChanges = _combatantLogger.GetStateChanges();
            
            AssertCombatStageLength(stateChanges, 0);
        }

        [Test]
        public void Negative_NullEntities_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _combatantLogger.LogCombatantChange(TICK, null!, [_targetCombatant], _directDamageStage, ABILITY_ID));
            Assert.Throws<ArgumentNullException>(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, null!, _directDamageStage, ABILITY_ID));
            Assert.Throws<EmptyCollectionException>(() => _combatantLogger.LogCombatantChange(TICK, _initiatingCombatant, [], _directDamageStage, ABILITY_ID));
        }
    }
}