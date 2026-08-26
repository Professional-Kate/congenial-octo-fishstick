using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
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
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityRepositoryMock;
        private Mock<ICombatQueueClear> _combatQueueClearMock;
        
        private CombatantEntity _friendlyCombatant;
        private CombatantAbilityEntity _friendlyAbility;
        
        private CombatantEntity _enemyCombatant;
        private CombatantAbilityEntity _enemyAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _combatantAbilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatQueueClearMock = new Mock<ICombatQueueClear>();
            
            _tearDownService = new TearDownService(_combatantRepositoryMock.Object, _combatantAbilityRepositoryMock.Object, _combatQueueClearMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _friendlyCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _friendlyCombatant.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            _friendlyAbility = TestCombatantAbilityEntityFactory.Create(_friendlyCombatant.CombatantID, 1);
            _friendlyAbility.AddComponent(new ReadyTickComponent { ReadyTick = 100d });
            
            _enemyCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2, TargetingType.ENEMY);
            _enemyCombatant.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            _enemyAbility = TestCombatantAbilityEntityFactory.Create(_enemyCombatant.CombatantID, 2);
            _enemyAbility.AddComponent(new ReadyTickComponent { ReadyTick = 200d });

            _combatantRepositoryMock.Reset();
            _combatantAbilityRepositoryMock.Reset();
            _combatQueueClearMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _combatantAbilityRepositoryMock.Verify();
            _combatantAbilityRepositoryMock.VerifyNoOtherCalls();
            _combatQueueClearMock.Verify();
            _combatQueueClearMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantRepositoryGetAll(params CombatantEntity[] entities)
        {
            _combatantRepositoryMock.Setup(library =>  library.GetAllParticipating()).Returns(entities).Verifiable();
        }

        private void SetupCombatantAbilityRepositoryContains(bool contains, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                _combatantAbilityRepositoryMock.Setup(library => library.Contains(combatantEntity.CombatantID)).Returns(contains).Verifiable();
            }
        }

        private void SetupCombatantAbilityRepositoryGetAll(CombatantEntity combatantEntity, params CombatantAbilityEntity[] combatantAbilityEntities)
        {
            _combatantAbilityRepositoryMock.Setup(library => library.GetAll(combatantEntity.CombatantID)).Returns(combatantAbilityEntities).Verifiable();
        }
        
        private void VerifyCombatQueueClear()
        {
            _combatQueueClearMock.Verify(library => library.Clear(), Times.Once);
        }

        private static void ChangeCombatantStats(HealthComponent healthComponent, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                combatantEntity.ReplaceComponent(healthComponent);
            }
        }

        private static void VerifyContainsFriendlyStatusComponent(bool shouldContain, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                Assert.That(combatantEntity.ContainsComponent<TargetingTypeComponent>(), Is.EqualTo(shouldContain));
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
                BaseHealthComponent baseHealthComponent = combatantEntity.GetComponent<BaseHealthComponent>();
                Assert.That(combatantEntity.GetComponent<HealthComponent>(), Is.EqualTo(new HealthComponent { Health = baseHealthComponent.Health }));
            }
        }

        private static void VerifyCombatParticipant(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                Assert.That(combatantEntity.ContainsComponent<CombatParticipantComponent>(), Is.False);
            }
        }
        
        private static void VerifyReadyTimeComponent(params CombatantAbilityEntity[] combatantAbilityEntities)
        {
            foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            { 
                Assert.That(combatantAbilityEntity.ContainsComponent<ReadyTickComponent>(), Is.False);
            }
        }

        private static void VerifyRetaliationComponentRemoved(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            { 
                Assert.That(combatantEntity.ContainsComponent<RetaliationComponent>(), Is.False);
            }
        }

        [Test]
        public void Positive_TearDownState_ChangesCombatants_ToBeforeCombatState_NoAbilities()
        {
            SetupCombatantRepositoryGetAll(_friendlyCombatant, _enemyCombatant);
            VerifyContainsFriendlyStatusComponent(true, _friendlyCombatant, _enemyCombatant);
            ChangeCombatantStats(new HealthComponent { Health = uint.MaxValue }, _friendlyCombatant, _enemyCombatant);
            SetupCombatantAbilityRepositoryContains(false, _friendlyCombatant, _enemyCombatant);
            _friendlyCombatant.AddComponent(new RetaliationComponent { Capacity = 1 });
            
            Assert.DoesNotThrow(() => _tearDownService.TearDownState());

            VerifyCombatQueueClear();
            VerifyContainsFriendlyStatusComponent(false, _friendlyCombatant, _enemyCombatant);
            VerifyLifeStatusComponent(_friendlyCombatant, _enemyCombatant);
            VerifyStatsComponent(_friendlyCombatant, _enemyCombatant);
            VerifyCombatParticipant(_friendlyCombatant, _enemyCombatant);
            VerifyRetaliationComponentRemoved(_friendlyCombatant);
        }

        [Test]
        public void Positive_TearDownState_ChangesCombatantsAbilities_ToBeforeCombatState()
        {
            SetupCombatantRepositoryGetAll(_friendlyCombatant, _enemyCombatant);
            SetupCombatantAbilityRepositoryContains(true, _friendlyCombatant, _enemyCombatant);
            SetupCombatantAbilityRepositoryGetAll(_friendlyCombatant, _friendlyAbility);
            SetupCombatantAbilityRepositoryGetAll(_enemyCombatant, _enemyAbility);
            
            Assert.DoesNotThrow(() => _tearDownService.TearDownState());
            
            VerifyCombatQueueClear();
            VerifyReadyTimeComponent(_friendlyAbility, _enemyAbility);
            VerifyCombatParticipant(_friendlyCombatant, _enemyCombatant);
        }

        [Test]
        public void Positive_ResetCombatants_Somehow_NoCombatants_NormalProcess()
        {
            SetupCombatantRepositoryGetAll();
            
            Assert.DoesNotThrow(() => _tearDownService.TearDownState());

            VerifyCombatQueueClear();
        }
    }
}