using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilityEventSchedulerTest
    {
        private AbilityEventScheduler _abilityEventScheduler;
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityEntityRepositoryMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<IAssetRepository<AbilityType, EventType>> _eventRepositoryMock;
        private Mock<ICombatQueue> _combatQueueMock;
        
        private CombatantEntity _combatantEntity;
        private CombatantAbilityEntity _combatantAbilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _eventRepositoryMock = new Mock<IAssetRepository<AbilityType, EventType>>();
            _combatQueueMock = new Mock<ICombatQueue>();
            
            _abilityEventScheduler = new AbilityEventScheduler(_combatantAbilityEntityRepositoryMock.Object, _combatantRepositoryMock.Object, _eventRepositoryMock.Object, _combatQueueMock.Object, new NumberAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(15);
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.CreateWithBaseComponents(_combatantEntity.CombatantID, AbilityType.STRONG_ATTACK);
            
            _combatantAbilityEntityRepositoryMock.Reset();
            _combatantRepositoryMock.Reset();
            _eventRepositoryMock.Reset();
            _combatQueueMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatantAbilityEntityRepositoryMock.Verify();
            _combatantAbilityEntityRepositoryMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _eventRepositoryMock.Verify();
            _eventRepositoryMock.VerifyNoOtherCalls();
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
        }

        private static void AddCastTimeComponent(CombatantAbilityEntity combatantAbilityEntity, double castTime)
        { 
            combatantAbilityEntity.AddComponent(new CastTimeComponent { CastTime = castTime });
        }

        private void SetupCombatantAbilityEntityGet(CombatantAbilityEntity combatantAbilityEntity)
        { 
            _combatantAbilityEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void SetupCombatantEntityGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private void SetupEventRepositoryGet(AbilityType abilityType, EventType eventType)
        {
            _eventRepositoryMock.Setup(library => library.Get(abilityType)).Returns(eventType).Verifiable();
        }

        private static CombatEvent CreateExpectedCombatEvent(CombatantAbilityEntity combatantAbilityEntity, double tick, EventType eventType)
        {
            return new CombatEvent
            {
                AbilityType = combatantAbilityEntity.AbilityType, 
                AttackerID = combatantAbilityEntity.CombatantID, 
                Tick = tick, 
                EventType = eventType
            };
        }

        private void VerifyQueueEnqueue(CombatEvent combatEvent)
        { 
            _combatQueueMock.Verify(library => library.Enqueue(combatEvent), Times.Once);
        }
 
        [Test]
        public void Positive_ScheduleEvent_HasCastTime_EnqueuesCastingEvent()
        {
            const double castTime = 120d; 
            AddCastTimeComponent(_combatantAbilityEntity, castTime);
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(0, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            CombatEvent expectedEvent = CreateExpectedCombatEvent(_combatantAbilityEntity, castTime, EventType.CASTING);
            _combatQueueMock.Verify(
                library => library.Enqueue(
                    It.Is<CombatEvent>(combatEvent => combatEvent.AbilityType == expectedEvent.AbilityType && combatEvent.AttackerID == expectedEvent.AttackerID)), Times.Once);
            
            VerifyMocks();
        }

        [Test]
        public void Positive_ScheduleEvent_NoCastTime_EnqueuesAbilityEvent()
        {
            const double forTick = 400d;
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupCombatantEntityGet(_combatantEntity);
            SetupEventRepositoryGet(_combatantAbilityEntity.AbilityType, EventType.DIRECT_DAMAGE);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(forTick, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_combatantAbilityEntity, forTick, EventType.DIRECT_DAMAGE));
            VerifyMocks();
        }
        
        [Test]
        public void Negative_ScheduleEvent_NoCastTime_EnqueuesAbilityEvent_ButAbilityNotFound_Throws()
        {
            const double forTick = 400d;
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            _eventRepositoryMock.Setup(library 
                => library.Get(_combatantAbilityEntity.AbilityType)).Throws(new NotFoundException<AbilityType>(_combatantAbilityEntity.AbilityType)).Verifiable();
            
            NotFoundException<AbilityType> exception = Assert.Throws<NotFoundException<AbilityType>>(() => _abilityEventScheduler.ScheduleEvent(forTick, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            Assert.That(exception.Key, Is.EqualTo(_combatantAbilityEntity.AbilityType));
            VerifyMocks();
        }
        
        [Test]
        public void Negative_ScheduleEvent_ZeroCombatantSpeed_Throws()
        {
            StatCard zeroSpeedStatCard = new() { Attack = 100, Health = 100, Speed = 0 };
            CombatantEntity zeroSpeedEntity = TestCombatantEntityFactory.CreateCombatantEntity(_combatantEntity.CombatantID, true, zeroSpeedStatCard);
            
            const double castTime = 120d; 
            AddCastTimeComponent(_combatantAbilityEntity, castTime);
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupCombatantEntityGet(zeroSpeedEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _abilityEventScheduler.ScheduleEvent(0, zeroSpeedEntity.CombatantID, _combatantAbilityEntity.AbilityType));

            Assert.That(exception.Source, Is.EqualTo("Speed"));
            VerifyMocks();
        }

        [Test]
        public void Negative_ScheduleEvent_HasCastTime_ZeroCastTime_Throws()
        {
            const double castTime = 0d; 
            AddCastTimeComponent(_combatantAbilityEntity, castTime);
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _abilityEventScheduler.ScheduleEvent(0, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            Assert.That(exception.Source, Is.EqualTo("CastTime"));
            VerifyMocks();
        }

        [Test]
        public void Positive_EnqueueAbilityEvent_EnqueuesAbilityEvent()
        {
            const double currentTick = 2345.2242d;
            SetupEventRepositoryGet(_combatantAbilityEntity.AbilityType, EventType.DIRECT_DAMAGE);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.EnqueueAbilityEvent(currentTick, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_combatantAbilityEntity, currentTick, EventType.DIRECT_DAMAGE));
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueAbilityEvent_CannotFindAbility_Throws()
        {
            const double currentTick = 2345.2242d;
            
            _eventRepositoryMock.Setup(library 
                => library.Get(_combatantAbilityEntity.AbilityType)).Throws(new NotFoundException<AbilityType>(_combatantAbilityEntity.AbilityType)).Verifiable();
            
            NotFoundException<AbilityType> exception = Assert.Throws<NotFoundException<AbilityType>>(() => _abilityEventScheduler.EnqueueAbilityEvent(currentTick, _combatantEntity.CombatantID, _combatantAbilityEntity.AbilityType));
            
            Assert.That(exception.Key, Is.EqualTo(_combatantAbilityEntity.AbilityType));
            VerifyMocks();
        }
    }
}