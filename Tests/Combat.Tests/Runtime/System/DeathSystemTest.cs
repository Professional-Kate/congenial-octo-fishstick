using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DeathSystemTest
    {
        private DeathSystem _deathSystem;
        private Mock<ICombatStateService> _combatStateServiceMock;
        
        private CombatantEntity _combatantEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatStateServiceMock = new Mock<ICombatStateService>();
            
            _deathSystem = new DeathSystem(_combatStateServiceMock.Object, new CombatantAssertion());
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
        }

        private void VerifyEvaluateCalled(CombatantEntity combatantEntity)
        {
            _combatStateServiceMock.Verify(library => library.Evaluate(combatantEntity), Times.Once);
        }

        private static void AssertEntityDead(CombatantEntity combatantEntity)
        {
            Assert.That(combatantEntity.GetComponent<LifeStatusComponent>().IsAlive, Is.False);
        }

        [Test]
        public void Positive_KillEntity_CombatNotOver_KillsEntity()
        {
            Assert.DoesNotThrow(() => _deathSystem.KillEntity(_combatantEntity));

            AssertEntityDead(_combatantEntity);
            VerifyEvaluateCalled(_combatantEntity);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_KillEntity_CombatOver_StoreNotUpdated()
        {
            Assert.DoesNotThrow(() => _deathSystem.KillEntity(_combatantEntity));

            AssertEntityDead(_combatantEntity);
            VerifyEvaluateCalled(_combatantEntity);
            VerifyMocks();
        }

        [Test]
        public void Negative_KillEntity_EntityAlreadyDead_Throws()
        {
            _combatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            Assert.Throws<CombatantDeadException>(() => _deathSystem.KillEntity(_combatantEntity));
            
            VerifyMocks();
        }
    }
}