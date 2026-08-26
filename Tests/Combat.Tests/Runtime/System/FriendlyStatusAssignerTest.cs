using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.ECS.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class FriendlyStatusAssignerTest
    {
        private FriendlyStatusAssigner _friendlyStatusAssigner;
        private Mock<ICombatantRepository> _repositoryMock;

        private CombatantEntity _friendlyEntity;
        private CombatantEntity _enemyEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            
            _friendlyStatusAssigner = new FriendlyStatusAssigner(_repositoryMock.Object, new CollectionAssertion(), new FoundAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _friendlyEntity = new CombatantEntity(new StatCard { Health = 10 }, new AgilityCard { Speed = 5, Initiative = 1 })
            {
                CombatantID = 1,
                CombatantType = CombatantType.HUMAN
            };
            
            _enemyEntity = new CombatantEntity(new StatCard { Health = 5 }, new AgilityCard { Speed = 8, Initiative = 1 })
            {
                CombatantID = 2,
                CombatantType = CombatantType.GOBLIN
            };
        }

        private void VerifyMocks()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryContains(byte id)
        {
            _repositoryMock.Setup(library => library.Contains(id)).Returns(true).Verifiable();
        }

        private void SetupRepositoryGet(CombatantEntity combatantEntity)
        { 
            _repositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private static void AssertEntityDoesNotHaveComponent(CombatantEntity combatantEntity)
        { 
            Assert.That(combatantEntity.ContainsComponent<TargetingTypeComponent>(), Is.False);
        }

        private static void AssertEntityHasComponent(CombatantEntity combatantEntity, TargetingType targetingType)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantEntity.ContainsComponent<TargetingTypeComponent>(), Is.True);
                Assert.That(combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType, Is.EqualTo(targetingType));
                Assert.That(combatantEntity.ContainsComponent<CombatParticipantComponent>(), Is.True);
            }
        }

        [Test]
        public void Positive_AssignFriendlyStatus_AddsNewComponents()
        {
            SetupRepositoryContains(_friendlyEntity.CombatantID);
            SetupRepositoryContains(_enemyEntity.CombatantID);
            SetupRepositoryGet(_friendlyEntity);
            SetupRepositoryGet(_enemyEntity);
            AssertEntityDoesNotHaveComponent(_friendlyEntity);
            AssertEntityDoesNotHaveComponent(_enemyEntity);
            
            Assert.DoesNotThrow(() => _friendlyStatusAssigner.AssignFriendlyStatus([_friendlyEntity.CombatantID], [_enemyEntity.CombatantID]));

            AssertEntityHasComponent(_friendlyEntity, TargetingType.FRIENDLY);
            AssertEntityHasComponent(_enemyEntity, TargetingType.ENEMY);
            VerifyMocks();
        }

        [Test]
        public void Negative_AssignFriendlyStatus_ComponentAlreadyExists_Throws()
        {
            SetupRepositoryContains(_friendlyEntity.CombatantID);
            SetupRepositoryGet(_friendlyEntity);
            AssertEntityDoesNotHaveComponent(_friendlyEntity);
            
            Assert.Throws<ComponentAlreadyExistsException>(() => _friendlyStatusAssigner.AssignFriendlyStatus([_friendlyEntity.CombatantID], [_friendlyEntity.CombatantID]));

            VerifyMocks();
        }

        [Test]
        public void Negative_AssignFriendlyStatus_BadFriendlyArray_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _friendlyStatusAssigner.AssignFriendlyStatus(null!, [_enemyEntity.CombatantID]));
            Assert.Throws<EmptyCollectionException>(() => _friendlyStatusAssigner.AssignFriendlyStatus([], [_enemyEntity.CombatantID]));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_AssignFriendlyStatus_BadEnemyArray_Throws()
        {
            SetupRepositoryContains(_friendlyEntity.CombatantID);
            SetupRepositoryContains(_enemyEntity.CombatantID);
            SetupRepositoryGet(_friendlyEntity);
            SetupRepositoryGet(_enemyEntity);
            
            Assert.Throws<ArgumentNullException>(() => _friendlyStatusAssigner.AssignFriendlyStatus([_friendlyEntity.CombatantID], null!));
            Assert.Throws<EmptyCollectionException>(() => _friendlyStatusAssigner.AssignFriendlyStatus([_enemyEntity.CombatantID], []));
            
            VerifyMocks();
        }
    }
}