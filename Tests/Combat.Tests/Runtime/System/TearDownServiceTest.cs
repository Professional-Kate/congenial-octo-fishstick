using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Queue.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class TearDownServiceTest
    {
        private TearDownService _tearDownService;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatQueueClear> _combatQueueClearMock;
        
        private CombatantEntity _friendlyCombatant;
        private CombatantEntity _enemyCombatant;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _combatQueueClearMock = new Mock<ICombatQueueClear>();
            
            _tearDownService = new TearDownService(_combatantRepositoryMock.Object, _combatQueueClearMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _friendlyCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _friendlyCombatant.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            _enemyCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2, false);
            _enemyCombatant.ReplaceComponent(new LifeStatusComponent { IsAlive = false });

            _combatantRepositoryMock.Reset();
            _combatQueueClearMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _combatQueueClearMock.Verify();
            _combatQueueClearMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantRepositoryGetAll(params CombatantEntity[] entities)
        {
            _combatantRepositoryMock.Setup(library =>  library.GetAll()).Returns(entities).Verifiable();
        }

        private void VerifyCombatQueueClear()
        {
            _combatQueueClearMock.Verify(library => library.Clear(), Times.Once);
        }

        private static void ChangeCombatantStats(StatsComponent statsComponent, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                combatantEntity.ReplaceComponent(statsComponent);
            }
        }

        private static void VerifyContainsFriendlyStatusComponent(bool shouldContain, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                Assert.That(combatantEntity.ContainsComponent<FriendlyStatusComponent>(), Is.EqualTo(shouldContain));
            }
        }
        
        private static void VerifyLifeStatusComponent(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            { 
                Assert.That(combatantEntity.GetComponent<LifeStatusComponent>().IsAlive, Is.True);
            }
        }
        
        private static void VerifyStatsComponent(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            { 
                BaseStatsComponent baseStatsComponent = combatantEntity.GetComponent<BaseStatsComponent>();
                Assert.That(combatantEntity.GetComponent<StatsComponent>(), Is.EqualTo(baseStatsComponent.GetStats));
            }
        }

        [Test]
        public void Positive_ResetCombatants_ChangesCombatants_ToBeforeCombatState()
        {
            SetupCombatantRepositoryGetAll(_friendlyCombatant, _enemyCombatant);
            VerifyContainsFriendlyStatusComponent(true, _friendlyCombatant, _enemyCombatant);
            ChangeCombatantStats(new StatsComponent { Health = uint.MaxValue }, _friendlyCombatant, _enemyCombatant);
            
            Assert.DoesNotThrow(() => _tearDownService.ResetCombatants());

            VerifyCombatQueueClear();
            VerifyContainsFriendlyStatusComponent(false, _friendlyCombatant, _enemyCombatant);
            VerifyLifeStatusComponent(_friendlyCombatant, _enemyCombatant);
            VerifyStatsComponent(_friendlyCombatant, _enemyCombatant);
        }

        [Test]
        public void Positive_ResetCombatants_Somehow_NoCombatants_NormalProcess()
        {
            SetupCombatantRepositoryGetAll();
            
            Assert.DoesNotThrow(() => _tearDownService.ResetCombatants());

            VerifyCombatQueueClear();
        }
    }
}