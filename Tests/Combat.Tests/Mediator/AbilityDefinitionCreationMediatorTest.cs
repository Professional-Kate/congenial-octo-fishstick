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
    public sealed class AbilityDefinitionCreationMediatorTest
    {
        private AbilityDefinitionCreationMediator _abilityDefinitionCreationMediator;
        private Mock<IAssetRepository<AbilityType, AbilityDefinition>> _repositoryMock;
        private Mock<IAbilityDefinitionFactory> _factoryMock;
        private Mock<IDispatchMany<AbilityDefinitionCreationResponse>> _responseDispatcherMock;
        
        private AbilityDefinitionCreation _slashDefinition;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<AbilityType, AbilityDefinition>>();
            _factoryMock = new Mock<IAbilityDefinitionFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityDefinitionCreationResponse>>();
            
            _abilityDefinitionCreationMediator = new AbilityDefinitionCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new AmountAssertion());
            
            _slashDefinition = new AbilityDefinitionCreation
            {
                AbilityType = AbilityType.SLASH,
                TargetingInformation = new TargetingInformation { TargetingType = TargetingType.GROUP, MaxTargets = 3 },
                Information = new Information { Name = "Slash!", Description = "Slash a group of enemies!" },
                Cooldown = 3,
                Damage = 5
            };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }
        
        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<AbilityDefinitionCreationResponse>>(collection => collection.Count == length)), Times.Once);
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void VerifyRepositoryContainsCalled(Times times)
        {
            _repositoryMock.Verify(library => library.Contains(It.IsAny<AbilityType>()), times);
        }
        
        private void VerifyRepositoryAddCalled(Times times)
        {
            _repositoryMock.Verify(library => library.Add(It.IsAny<AbilityType>(), It.IsAny<AbilityDefinition>()), times);
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleCommand_CreatesDefinition()
        { 
            Assert.DoesNotThrow(() => _abilityDefinitionCreationMediator.HandleMessages([_slashDefinition]));
            
            VerifyDispatcherCalled(1);
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryAddCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleCommands_CreatesDefinition()
        {
            AbilityDefinitionCreation stabDefinition = _slashDefinition with { AbilityType = AbilityType.STAB };
            
            Assert.DoesNotThrow(() => _abilityDefinitionCreationMediator.HandleMessages([_slashDefinition, stabDefinition]));
            
            VerifyDispatcherCalled(2);
            VerifyRepositoryContainsCalled(Times.Exactly(2));
            VerifyRepositoryAddCalled(Times.Exactly(2));
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Positive_HandleMessages_ZeroCooldown_CreatesDefinition()
        {
            AbilityDefinitionCreation zeroCooldownDefinition = _slashDefinition with { Cooldown = 0 };
            
            Assert.DoesNotThrow(() => _abilityDefinitionCreationMediator.HandleMessages([zeroCooldownDefinition]));
            
            VerifyDispatcherCalled(1);
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryAddCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _abilityDefinitionCreationMediator.HandleMessages([]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _abilityDefinitionCreationMediator.HandleMessages(null!));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateAbilityType_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_slashDefinition.AbilityType)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _abilityDefinitionCreationMediator.HandleMessages([_slashDefinition]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_ZeroDamage_Throws()
        {
            AbilityDefinitionCreation zeroDamageDefinition = _slashDefinition with { Damage = 0 };
            
            Assert.Throws<AmountZeroException>(() => _abilityDefinitionCreationMediator.HandleMessages([zeroDamageDefinition]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_ZeroTargets_Throws()
        {
            AbilityDefinitionCreation zeroTargetsDefinition = _slashDefinition with { TargetingInformation = new TargetingInformation { TargetingType = TargetingType.STRONG, MaxTargets = 0 }};
            
            Assert.Throws<AmountZeroException>(() => _abilityDefinitionCreationMediator.HandleMessages([zeroTargetsDefinition]));
            
            _responseDispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }
    }
}