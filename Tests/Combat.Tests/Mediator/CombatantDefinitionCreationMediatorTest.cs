using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public class CombatantDefinitionCreationMediatorTest
    {
        private CombatantDefinitionCreationMediator _combatantDefinitionCreationMediator;
        private Mock<IAssetRepository<CombatantType, CombatantDefinition>> _definitionRepositoryMock;
        private Mock<ICombatantDefinitionFactory> _definitionFactoryMock;
        private Mock<IDispatchMany<CombatantDefinitionCreationResponse>> _responseDispatcherMock;

        private CombatantDefinitionCreation _slimeDefinitionCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _definitionRepositoryMock = new Mock<IAssetRepository<CombatantType, CombatantDefinition>>();
            _definitionFactoryMock = new Mock<ICombatantDefinitionFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantDefinitionCreationResponse>>();
            
            _combatantDefinitionCreationMediator = new CombatantDefinitionCreationMediator(_definitionRepositoryMock.Object, _definitionFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new CombatantStatsAssertion(new AmountAssertion()));
            
            _slimeDefinitionCreation = new CombatantDefinitionCreation
            {
                CombatantType = CombatantType.SLIME,
                CombatantStats = new CombatantStats { Attack = 1, Health = 1, Speed = 1 },
                Information = new Information { Name = "", Description = "" }
            };
        }

        [SetUp]
        public void Setup()
        {
            _definitionRepositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<CombatantDefinitionCreationResponse>>(collection => collection.Count == length)));
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void VerifyRepositoryContainsCalled(Times times)
        {
            _definitionRepositoryMock.Verify(library => library.Contains(It.IsAny<CombatantType>()), times);
        }
        
        private void VerifyRepositoryAddCalled(Times times)
        {
            _definitionRepositoryMock.Verify(library => library.Add(It.IsAny<CombatantType>(), It.IsAny<CombatantDefinition>()), times);
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _definitionRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleCommand_CreatesDefinition()
        {
            Assert.DoesNotThrow(() => _combatantDefinitionCreationMediator.HandleMessages([_slimeDefinitionCreation]));

            VerifyDispatcherCalled(1);
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryAddCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_CreatesDefinitions()
        {
            CombatantDefinitionCreation wolfDefinition = _slimeDefinitionCreation with { CombatantType = CombatantType.WOLF };
            
            Assert.DoesNotThrow(() => _combatantDefinitionCreationMediator.HandleMessages([_slimeDefinitionCreation, wolfDefinition]));

            VerifyDispatcherCalled(2);
            VerifyRepositoryContainsCalled(Times.Exactly(2));
            VerifyRepositoryAddCalled(Times.Exactly(2));
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatantDefinitionCreationMediator.HandleMessages([]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _combatantDefinitionCreationMediator.HandleMessages(null!));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateCombatantType_Throws()
        {
            _definitionRepositoryMock.Setup(library => library.Contains(_slimeDefinitionCreation.CombatantType)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _combatantDefinitionCreationMediator.HandleMessages([_slimeDefinitionCreation]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_BadCombatantStats_Throws()
        {
            CombatantDefinitionCreation badStatCreation = _slimeDefinitionCreation with { CombatantStats = new CombatantStats { Attack = 0, Health = 0,  Speed = 0 }};
            
            Assert.Throws<AmountZeroException>(() => _combatantDefinitionCreationMediator.HandleMessages([badStatCreation]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
            
        }
    }
}