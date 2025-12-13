using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class ArenaCreationMediatorTest
    {
        private ArenaCreationMediator _arenaCreationMediator;
        private Mock<IStateRepository<ArenaType, Arena>> _repositoryMock;
        private Mock<IArenaFactory> _arenaFactoryMock;
        private Mock<IDispatchMany<ArenaCreationResponse>> _dispatcherMock;
        
        private ArenaCreation _arenaCreation;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IStateRepository<ArenaType, Arena>>();
            _arenaFactoryMock = new Mock<IArenaFactory>();
            _dispatcherMock = new Mock<IDispatchMany<ArenaCreationResponse>>();
            
            _arenaCreationMediator = new ArenaCreationMediator(_repositoryMock.Object, _arenaFactoryMock.Object, _dispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new LevelAssertion());
            
            _arenaCreation = new ArenaCreation { ArenaType = ArenaType.FIELD, Information = new Information { Name = "", Description = "" }, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 }};
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _dispatcherMock.Reset();
            _arenaFactoryMock.Reset();
        }

        private void SetupArenaFactory(ArenaCreation arenaCreation)
        {
            ReadOnlyLevelable readOnlyLevelable = arenaCreation.ReadOnlyLevelable;
            
            Arena arena = new()
            {
                ArenaType = arenaCreation.ArenaType,
                Information = arenaCreation.Information,
                Levelable = new Levelable(readOnlyLevelable.Level, readOnlyLevelable.Experience, readOnlyLevelable.NextLevelExperience, readOnlyLevelable.ExperiencePerAction)
            };
            
            _arenaFactoryMock.Setup(library => library.Create(arenaCreation)).Returns(arena);
        }

        private void VerifyDispatcherCalled(int length)
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<ArenaCreationResponse>>(collection => collection.Count == length)), Times.Once);
            _dispatcherMock.VerifyNoOtherCalls();
        }

        private void VerifyRepositoryContainsCalled(Times times)
        {
            _repositoryMock.Verify(library => library.Contains(It.IsAny<ArenaType>()), times);
        }
        
        private void VerifyRepositoryAddCalled(Times times)
        {
            _repositoryMock.Verify(library => library.Add(It.IsAny<ArenaType>(), It.IsAny<Arena>()), times);
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleCommand_CreatesArena_DispatchesResponse()
        { 
            SetupArenaFactory(_arenaCreation);
            
            Assert.DoesNotThrow(() => _arenaCreationMediator.HandleMessages([_arenaCreation]));

            VerifyDispatcherCalled(1);
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryAddCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_CreatesArena_DispatchesResponses()
        {
            ArenaCreation arenaCreation = _arenaCreation with { ArenaType = ArenaType.CAVE };
            
            SetupArenaFactory(_arenaCreation);
            SetupArenaFactory(arenaCreation);
            
            Assert.DoesNotThrow(() => _arenaCreationMediator.HandleMessages([_arenaCreation, arenaCreation]));

            VerifyDispatcherCalled(2);
            VerifyRepositoryContainsCalled(Times.Exactly(2));
            VerifyRepositoryAddCalled(Times.Exactly(2));
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_ArenaAtMaxLevel_CreatesArena_DispatchesResponse()
        {
            ArenaCreation maxLevelCreation = _arenaCreation with { ReadOnlyLevelable = _arenaCreation.ReadOnlyLevelable with { Level = LevelConstants.MAX_LEVEL } };
            
            SetupArenaFactory(maxLevelCreation);
            
            Assert.DoesNotThrow(() => _arenaCreationMediator.HandleMessages([maxLevelCreation]));
            
            VerifyDispatcherCalled(1);
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryAddCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _arenaCreationMediator.HandleMessages([]));

            _dispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _arenaCreationMediator.HandleMessages(null!));

            _dispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateArenaType_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_arenaCreation.ArenaType)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _arenaCreationMediator.HandleMessages([_arenaCreation]));

            _dispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_OverMaxLevel_Throws()
        {
            ArenaCreation overMaxLevelCreation = _arenaCreation with { ReadOnlyLevelable = _arenaCreation.ReadOnlyLevelable with { Level = LevelConstants.MAX_LEVEL + 1 } };
            
            SetupArenaFactory(overMaxLevelCreation);
            
            Assert.Throws<MaxLevelException>(() => _arenaCreationMediator.HandleMessages([overMaxLevelCreation]));
            
            _dispatcherMock.VerifyNoOtherCalls();
            VerifyRepositoryContainsCalled(Times.Once());
            VerifyRepositoryNoOtherCalls();
        }
    }
}