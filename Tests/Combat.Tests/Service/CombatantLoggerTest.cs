using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Service.Logging;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatantLoggerTest
    {
        private CombatantLogger _combatantLogger;

        private const double TICK = 100d;
        private CombatantEntity _combatantEntity;
        private CombatantCreation _combatantCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF);
        }
        
        [SetUp]
        public void Setup()
        {
            _combatantLogger = new CombatantLogger(new ObjectNullAssertion());
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _combatantCreation);
        }

        private static void AssertStateChangesLength(IReadOnlyList<CombatantStateChange> stateChanges, int expectedLength)
        {
            Assert.That(stateChanges, Has.Count.EqualTo(expectedLength));
        }

        private static void AssertStateChange(CombatantStateChange combatantStateChange, CombatantEntity combatantEntity)
        {
            Assert.Multiple(() =>
            {
                Assert.That(combatantStateChange.CombatantID, Is.EqualTo(combatantEntity.CombatantID));
                Assert.That(combatantStateChange.IsAlive, Is.EqualTo(combatantEntity.GetComponent<LifeStatusComponent>().IsAlive));
                Assert.That(combatantStateChange.IsFriendly, Is.EqualTo(combatantEntity.GetComponent<FriendlyStatusComponent>().IsFriendly));
                
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                StatCard stateChangeStats = combatantStateChange.CombatantCreation.StatCard;
                
                Assert.That(stateChangeStats.Attack, Is.EqualTo(combatantStatsComponent.Attack));
                Assert.That(stateChangeStats.Health, Is.EqualTo(combatantStatsComponent.Health));
                Assert.That(stateChangeStats.Speed, Is.EqualTo(combatantStatsComponent.Speed));
            });
        }

        [Test]
        public void Positive_LogCombatantChange_LogsEntity()
        {
            _combatantEntity.UpdateLifeStatus(false);
            
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(_combatantEntity, 2, AbilityType.STRONG_ATTACK, 10, TICK));

            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            AssertStateChangesLength(stateChanges, 1);
            AssertStateChange(stateChanges[0], _combatantEntity);
        }
        
        [Test]
        public void Positive_LogCombatantChange_LogEntityTwice_LogsEntity()
        {
            _combatantEntity.UpdateCombatantStats(new CombatantStatsComponent { Attack = 4, Health = 5, Speed = 3 });
            
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(_combatantEntity, 2, AbilityType.STRONG_ATTACK, 10, TICK));
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(_combatantEntity, 2, AbilityType.STRONG_ATTACK, 10, TICK));

            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            AssertStateChangesLength(stateChanges, 2);
            AssertStateChange(stateChanges[0], _combatantEntity);
            AssertStateChange(stateChanges[1], _combatantEntity);
        }

        [Test]
        public void Positive_GetStateChanges_NoStateChanges_ReturnsEmptyArray()
        {
            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            AssertStateChangesLength(stateChanges, 0);
        }

        [Test]
        public void Positive_GetStateChanges_MultipleTimes_ReturnsSameArray()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(_combatantEntity, 2, AbilityType.STRONG_ATTACK, 10, TICK));
            
            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            Assert.That(stateChanges, Is.EquivalentTo(_combatantLogger.GetStateChanges()));
        }

        [Test]
        public void Positive_ClearStateChanges_ClearsStates()
        {
            Assert.DoesNotThrow(() => _combatantLogger.LogCombatantChange(_combatantEntity, 2, AbilityType.STRONG_ATTACK, 10, TICK));
            AssertStateChangesLength(_combatantLogger.GetStateChanges(), 1);
            
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            AssertStateChangesLength( _combatantLogger.GetStateChanges(), 0);
        }

        [Test]
        public void Positive_ClearStateChanges_NoChanges_DoesNothing()
        {
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            Assert.DoesNotThrow(() => _combatantLogger.ClearStateChanges());
            
            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            AssertStateChangesLength(stateChanges, 0);
        }

        [Test]
        public void Negative_LogCombatantChange_NullEntity_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _combatantLogger.LogCombatantChange(null!, 2, AbilityType.STRONG_ATTACK, 10, TICK));
            
            IReadOnlyList<CombatantStateChange> stateChanges = _combatantLogger.GetStateChanges();
            AssertStateChangesLength(stateChanges, 0);
        }
    }
}