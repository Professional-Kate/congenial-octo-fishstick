using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
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
    public sealed class StatConversionCreationMediatorTest
    {
        private StatConversionCreationMediator _statConversionCreationMediator;
        private Mock<IIncrementalRepository<StatConversionCreation>> _statConversionRepositoryMock;
        private Mock<IDispatchMany<StatConversionCreationResponse>> _responseDispatcherMock;

        private readonly StatConversionCreationResponse _statConversionCreationResponse = new()
        {
            StatConversionID = 1,
            StatConversionCreation = new StatConversionCreation
            {
                TargetStatType = StatType.HEALTH,
                SourceStatType = StatType.BASE_HEALTH,
                SourceStatInterval = 3,
                StatOperation = StatOperation.ADDITIVE,
                TargetStatModifier = 1
            }
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _statConversionRepositoryMock = new Mock<IIncrementalRepository<StatConversionCreation>>();
            _responseDispatcherMock = new Mock<IDispatchMany<StatConversionCreationResponse>>();
            
            _statConversionCreationMediator = new StatConversionCreationMediator(_statConversionRepositoryMock.Object, _responseDispatcherMock.Object,  new CollectionAssertion(), new NumberAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _statConversionRepositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _statConversionRepositoryMock.Verify();
            _statConversionRepositoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryAdd(StatConversionCreation statConversionCreation, byte expectedID)
        {
            _statConversionRepositoryMock.Setup(library => library.Add(statConversionCreation)).Returns(expectedID).Verifiable();
        }

        private void VerifyDispatch(StatConversionCreationResponse[] statConversionResponses)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(statConversionResponses), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_AddsToRepository()
        { 
            SetupRepositoryAdd(_statConversionCreationResponse.StatConversionCreation, _statConversionCreationResponse.StatConversionID);
            
            Assert.DoesNotThrow(() => _statConversionCreationMediator.HandleMessages([_statConversionCreationResponse.StatConversionCreation]));
            
            VerifyDispatch([_statConversionCreationResponse]);
        }

        [Test]
        public void Positive_HandleMessages_CanAddDuplicates()
        {
            SetupRepositoryAdd(_statConversionCreationResponse.StatConversionCreation, _statConversionCreationResponse.StatConversionID);
            
            Assert.DoesNotThrow(() => _statConversionCreationMediator.HandleMessages([_statConversionCreationResponse.StatConversionCreation, _statConversionCreationResponse.StatConversionCreation]));
            
            VerifyDispatch([_statConversionCreationResponse, _statConversionCreationResponse]);
        }

        [Test]
        public void Negative_HandleMessages_BadCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _statConversionCreationMediator.HandleMessages([]));
            Assert.Throws<ArgumentNullException>(() => _statConversionCreationMediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessages_NumbersZero_Throws()
        {
            StatConversionCreation goodStatConversionCreation = new()
            {
                TargetStatType = StatType.HEALTH,
                SourceStatType = StatType.BASE_HEALTH,
                SourceStatInterval = 3,
                StatOperation = StatOperation.ADDITIVE,
                TargetStatModifier = 0
            };
            
            Assert.Throws<NumberZeroException>(() => _statConversionCreationMediator.HandleMessages([goodStatConversionCreation with { TargetStatModifier = 0 }]));
            Assert.Throws<NegativeNumberException>(() => _statConversionCreationMediator.HandleMessages([goodStatConversionCreation with { SourceStatInterval = -1 }]));
            Assert.Throws<NumberZeroException>(() => _statConversionCreationMediator.HandleMessages([goodStatConversionCreation with { SourceStatInterval = 0 }]));
        }
    }
}