using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DeathSystemTest
    {
        private DeathSystem _deathSystem;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        
        private CombatantEntity _combatantEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            
            _deathSystem = new DeathSystem(_combatStateServiceMock.Object, _combatantStoreServiceMock.Object, new CombatantAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(1);
        }

        private void VerifyMocks()
        {
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
        }

        private void VerifyEvaluateCalled(CombatantEntity combatantEntity)
        {
            _combatStateServiceMock.Verify(library => library.Evaluate(combatantEntity), Times.Once);
        }

        private void SetupIsCombatOver(bool isCombatOver)
        {
            _combatStateServiceMock.Setup(library => library.IsCombatOver).Returns(isCombatOver).Verifiable();
        }

        private void VerifyRegisterCombatantDeath(CombatantEntity combatantEntity)
        {
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantDeath(combatantEntity), Times.Once);
        }

        private static void AssertEntityDead(CombatantEntity combatantEntity)
        {
            Assert.That(combatantEntity.GetComponent<LifeStatusComponent>().IsAlive, Is.False);
        }

        [Test]
        public void Positive_KillEntity_CombatNotOver_KillsEntity()
        {
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _deathSystem.KillEntity(_combatantEntity));

            AssertEntityDead(_combatantEntity);
            VerifyRegisterCombatantDeath(_combatantEntity);
            VerifyEvaluateCalled(_combatantEntity);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_KillEntity_CombatOver_StoreNotUpdated()
        {
            SetupIsCombatOver(true);
            
            Assert.DoesNotThrow(() => _deathSystem.KillEntity(_combatantEntity));

            AssertEntityDead(_combatantEntity);
            VerifyEvaluateCalled(_combatantEntity);
            VerifyMocks();
        }

        [Test]
        public void Negative_KillEntity_EntityAlreadyDead_Throws()
        {
            _combatantEntity.UpdateLifeStatus(false);
            
            Assert.Throws<CombatantDeadException>(() => _deathSystem.KillEntity(_combatantEntity));
            
            VerifyMocks();
        }
    }
}