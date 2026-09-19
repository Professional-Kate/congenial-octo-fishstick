using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Combat.Stat.Mediator;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Stat.Mediator
{
    [TestFixture]
    public sealed class StatCreationMediatorTest
    {
        private StatCreationMediator _statCreationMediator;
        private Mock<IIncrementalRepository<StatCreation>> _repositoryMock;
        private Mock<IDispatchMany<StatCreationResponse>> _responseDispatcherMock;

        private readonly StatCreation _statCreation = new()
        {
            InitialValue = 10
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IIncrementalRepository<StatCreation>>();
            _responseDispatcherMock = new Mock<IDispatchMany<StatCreationResponse>>();
            
            _statCreationMediator = new StatCreationMediator(_repositoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void SetupRepositoryAdd(params StatCreation[] statCreations)
        {
            for (byte i = 0; i < statCreations.Length; i++)
            {
                StatCreation statCreation = statCreations[i];
                _repositoryMock.Setup(library => library.Add(statCreation)).Returns(i).Verifiable();
            }
        }

        private static StatCreationResponse[] CreateExpectedResponses(StatCreation[] statCreations)
        {
            StatCreationResponse[] statCreationResponses = new StatCreationResponse[statCreations.Length];
            for (byte i = 0; i < statCreations.Length; i++)
            {
                StatCreation statCreation = statCreations[i];
                statCreationResponses[i] = new StatCreationResponse { StatCreation = statCreation, StatID = i };
            }

            return statCreationResponses;
        }

        private void VerifyDispatch(params StatCreation[] statCreations)
        { 
            _responseDispatcherMock.Verify(library => library.Dispatch(CreateExpectedResponses(statCreations)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_AddsToRepository()
        {
            SetupRepositoryAdd(_statCreation);
            
            Assert.DoesNotThrow(() => _statCreationMediator.HandleMessages([_statCreation]));
            
            VerifyDispatch(_statCreation);
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleMessages_AddsToRepository()
        {
            StatCreation lowValueCreation = new() { InitialValue = 0 };
            StatCreation highValueCreation = new() { InitialValue = 20 };
            
            SetupRepositoryAdd(_statCreation, lowValueCreation, highValueCreation);
            
            Assert.DoesNotThrow(() => _statCreationMediator.HandleMessages([_statCreation, lowValueCreation, highValueCreation]));
            
            VerifyDispatch(_statCreation, lowValueCreation, highValueCreation);
        }

        [Test]
        public void Negative_HandleMessages_BadCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _statCreationMediator.HandleMessages([]));
            Assert.Throws<ArgumentNullException>(() => _statCreationMediator.HandleMessages(null!));
        }
    }
}