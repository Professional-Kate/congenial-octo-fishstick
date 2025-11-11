using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service;
using IdelPog.Currency.Service.Interface;
using Moq;

namespace IdelPog.Currency.Tests.Service
{
    [TestFixture]
    public sealed class CurrencyUpdateServiceTest
    {
        private ICurrencyUpdateService _currencyUpdateService;
        private Mock<ICurrencyService> _currencyServiceMock;
        private Mock<IStateRepository<CurrencyType,Contracts.Currency>> _currencyRepositoryMock;
        private Mock<ICurrencyUpdateResponseFactory> _currencyUpdateResponseFactoryMock;
        private Mock<ICurrencyUpdateSummarizer> _currencyUpdateSummarizerMock;

        private Contracts.Currency _goldCurrency;
        private CurrencyUpdate _addGoldUpdate;
        private CurrencyUpdateResponse _goldUpdateResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyRepositoryMock = new Mock<IStateRepository<CurrencyType, Contracts.Currency>>();
            _currencyUpdateResponseFactoryMock = new Mock<ICurrencyUpdateResponseFactory>();
            _currencyUpdateSummarizerMock = new Mock<ICurrencyUpdateSummarizer>();
            
            _currencyUpdateService = new CurrencyUpdateService(_currencyServiceMock.Object, _currencyRepositoryMock.Object, new CollectionAssertion(), new FoundAssertion(), _currencyUpdateResponseFactoryMock.Object, _currencyUpdateSummarizerMock.Object);

            _addGoldUpdate = new CurrencyUpdate { CurrencyType = CurrencyType.GOLD, Amount = 1, ActionType = ActionType.ADD };
            _goldUpdateResponse = new CurrencyUpdateResponse { CurrencyType = CurrencyType.GOLD, CurrencyAmount = 1 };
        }

        [SetUp]
        public void Setup()
        { 
            _goldCurrency = new Contracts.Currency(CurrencyType.GOLD, 0);
            _currencyRepositoryMock.Reset();
            _currencyServiceMock.Reset();
            _currencyUpdateSummarizerMock.Reset();
        }

        private void SetupSummarizer(CurrencyUpdate[] updates, params CurrencyUpdate[] expected)
        {
            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(updates)).Returns(expected);
        }

        private void SetupRepository(Contracts.Currency currency)
        {
            _currencyRepositoryMock.Setup(library => library.Contains(currency.CurrencyType)).Returns(true);
            _currencyRepositoryMock.Setup(library => library.Get(currency.CurrencyType)).Returns(currency);
        }

        private void SetupResponseFactory(Contracts.Currency[] currencies, params CurrencyUpdateResponse[] responses)
        {
            _currencyUpdateResponseFactoryMock.Setup(library => library.CreateFrom(currencies)).Returns(responses);
        }

        private void VerifyRepositoryCalled(CurrencyType currencyType, Times times)
        { 
            _currencyRepositoryMock.Verify(library => library.Contains(currencyType), times);
            _currencyRepositoryMock.Verify(library => library.Get(currencyType), times);
            _currencyRepositoryMock.Verify(library => library.Update(currencyType, It.Is<Contracts.Currency>(currency => currency.CurrencyType == currencyType)), times);
            _currencyRepositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyCurrencyServiceAddAmount(Contracts.Currency currency, uint amount, Times times)
        { 
            _currencyServiceMock.Verify(library => library.AddAmount(currency, amount), times);
        }
        
        private void VerifyCurrencyServiceRemoveAmount(Contracts.Currency currency, uint amount, Times times)
        { 
            _currencyServiceMock.Verify(library => library.RemoveAmount(currency, amount), times);
        }

        private void VerifyCurrencyServiceNoOtherCalls()
        { 
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        private static void AssertResponse(CurrencyUpdate update, CurrencyUpdateResponse response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.CurrencyAmount, Is.EqualTo(update.Amount));
                Assert.That(response.CurrencyType, Is.EqualTo(update.CurrencyType));
            });
        }

        [Test]
        public void Positive_ApplyUpdates_UpdatesCurrencyCorrectly()
        {
            SetupSummarizer([_addGoldUpdate], _addGoldUpdate);
            SetupRepository(_goldCurrency);
            SetupResponseFactory([_goldCurrency], _goldUpdateResponse);
                
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateService.ApplyUpdates([_addGoldUpdate]);
            
            Assert.That(responses, Has.Count.EqualTo(1));
            AssertResponse(_addGoldUpdate, responses[0]);
            VerifyRepositoryCalled(CurrencyType.GOLD, Times.Once());
            
            VerifyCurrencyServiceAddAmount(_goldCurrency, 1, Times.Once());
            VerifyCurrencyServiceNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_SummerizesUpdates_SingleResponse()
        {
            SetupSummarizer([_addGoldUpdate, _addGoldUpdate, _addGoldUpdate], _addGoldUpdate with { Amount = 3 });
            SetupRepository(_goldCurrency);
            SetupResponseFactory([_goldCurrency], _goldUpdateResponse with { CurrencyAmount = 3 });
            
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateService.ApplyUpdates([_addGoldUpdate, _addGoldUpdate, _addGoldUpdate]);
            
            Assert.That(responses, Has.Count.EqualTo(1));
            AssertResponse(_addGoldUpdate with { Amount = 3 }, responses[0]);
            VerifyRepositoryCalled(CurrencyType.GOLD, Times.Once());
            
            VerifyCurrencyServiceAddAmount(_goldCurrency, 3, Times.Once());
            VerifyCurrencyServiceNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_RemoveUpdate_RemovesCurrency()
        {
            CurrencyUpdate removeGoldUpdate = _addGoldUpdate with { ActionType = ActionType.REMOVE };
            
            SetupSummarizer([removeGoldUpdate], removeGoldUpdate);
            SetupRepository(_goldCurrency);
            SetupResponseFactory([_goldCurrency], _goldUpdateResponse);
                
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateService.ApplyUpdates([removeGoldUpdate]);
            
            Assert.That(responses, Has.Count.EqualTo(1));
            AssertResponse(removeGoldUpdate, responses[0]);
            VerifyRepositoryCalled(CurrencyType.GOLD, Times.Once());
            
            VerifyCurrencyServiceRemoveAmount(_goldCurrency, 1, Times.Once());
            VerifyCurrencyServiceNoOtherCalls();
        }

        [Test]
        public void Negative_ApplyUpdates_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _currencyUpdateService.ApplyUpdates([]));
            
            _currencyUpdateSummarizerMock.VerifyNoOtherCalls();
            _currencyRepositoryMock.VerifyNoOtherCalls();
            VerifyCurrencyServiceNoOtherCalls();
        }
        
        [Test]
        public void Negative_ApplyUpdates_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateService.ApplyUpdates(null!));
            
            _currencyUpdateSummarizerMock.VerifyNoOtherCalls();
            _currencyRepositoryMock.VerifyNoOtherCalls();
            VerifyCurrencyServiceNoOtherCalls();
        }

        [Test]
        public void Negative_ApplyUpdates_GetSummary_ReturnsNothing_Throws()
        {
            CurrencyUpdate[] updates = [_addGoldUpdate];
            SetupSummarizer(updates);
            
            Assert.Throws<EmptyCollectionException>(() => _currencyUpdateService.ApplyUpdates(updates));
            
            _currencyUpdateSummarizerMock.Verify(library => library.GetSummary(updates), Times.Once);
            _currencyUpdateSummarizerMock.VerifyNoOtherCalls();
            _currencyRepositoryMock.VerifyNoOtherCalls();
            VerifyCurrencyServiceNoOtherCalls();
        }

        [Test]
        public void Negative_ApplyUpdates_CurrencyDoesNotExist_Throws()
        {
            SetupSummarizer([_addGoldUpdate], _addGoldUpdate);
            
            Assert.Throws<NotFoundException<CurrencyType>>(() => _currencyUpdateService.ApplyUpdates([_addGoldUpdate]));
            
            _currencyRepositoryMock.Verify(library => library.Contains(CurrencyType.GOLD), Times.Once);
            _currencyRepositoryMock.VerifyNoOtherCalls();
            VerifyCurrencyServiceNoOtherCalls();
        }
    }
}