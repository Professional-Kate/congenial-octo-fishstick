using IdelPog.Combat.Assertion;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Exceptions;
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
            _combatantEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);
            _combatStateServiceMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
        }

        private void VerifyEvaluateCalled()
        {
            _combatStateServiceMock.Verify(library => library.Evaluate(), Times.Once);
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
            VerifyEvaluateCalled();
            VerifyMocks();
        }
        
        [Test]
        public void Positive_KillEntity_CombatOver_StoreNotUpdated()
        {
            Assert.DoesNotThrow(() => _deathSystem.KillEntity(_combatantEntity));

            AssertEntityDead(_combatantEntity);
            VerifyEvaluateCalled();
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