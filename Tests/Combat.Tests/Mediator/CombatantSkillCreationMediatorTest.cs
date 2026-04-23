using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class CombatantSkillCreationMediatorTest
    {
        private CombatantSkillCreationMediator _mediator;
        private Mock<IAssetRepository<SkillType, SkillEntity>> _repositoryMock; 
        private Mock<ISkillEntityFactory> _factoryMock;
        private Mock<IDispatchMany<CombatantSkillCreationResponse>> _responseDispatcherMock;

        private CombatantSkillCreation _combatantSkillCreation;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<SkillType, SkillEntity>>();
            _factoryMock = new Mock<ISkillEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantSkillCreationResponse>>();
            
            _mediator = new CombatantSkillCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion());
            _combatantSkillCreation = CombatantSkillCreationFactory.Create(SkillType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyMocks()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            _factoryMock.Verify();
            _factoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void AssertRepositoryContains(CombatantSkillCreation combatantSkillCreation)
        {
            _repositoryMock.Verify(library => library.Contains(combatantSkillCreation.SkillType), Times.Once);
        }
        
        private void AssertRepositoryAdd(CombatantSkillCreation combatantSkillCreation)
        {
            _repositoryMock.Verify(library => library.Add(combatantSkillCreation.SkillType, It.IsAny<SkillEntity>()), Times.Once);
        }

        private void VerifyFactoryCalled(CombatantSkillCreation combatantSkillCreation)
        {
            _factoryMock.Verify(library => library.CreateSkillEntity(combatantSkillCreation), Times.Once);
        }

        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantSkillCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantSkillCreation]));

            VerifyFactoryCalled(_combatantSkillCreation);
            AssertRepositoryContains(_combatantSkillCreation);
            AssertRepositoryAdd(_combatantSkillCreation);
            VerifyDispatcherCalled(1);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesNewEntities()
        {
            // TODO: update when we add another SkillType :)
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantSkillCreation, _combatantSkillCreation with { SkillType = (SkillType) 2 }]));

            VerifyFactoryCalled(_combatantSkillCreation);
            VerifyFactoryCalled(_combatantSkillCreation with { SkillType = (SkillType) 2 });
            AssertRepositoryContains(_combatantSkillCreation);
            AssertRepositoryContains(_combatantSkillCreation with { SkillType = (SkillType) 2 });
            AssertRepositoryAdd(_combatantSkillCreation);
            AssertRepositoryAdd(_combatantSkillCreation with { SkillType = (SkillType) 2 });
            VerifyDispatcherCalled(2);
            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateSkillType_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_combatantSkillCreation.SkillType)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _mediator.HandleMessages([_combatantSkillCreation]));
            
            VerifyMocks();
            
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));
            
            VerifyMocks();
        }
    }
}