using IdelPog.Constants;
using IdelPog.Controller;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Integration
{
    [TestFixture]
    public class CurrencyFlowTest
    {
        private readonly ICurrencyController _currencyController = CurrencyController.GetInstance();
        private readonly IRepository<CurrencyType, Currency> _repository = Repository<CurrencyType, Currency>.GetInstance();

        private Currency _peopleCurrency;
        private Currency _goldCurrency;
        private const int AMOUNT = 10;

        private CurrencyTrade _addPeopleTrade { get; set; }
        private CurrencyTrade _removePeopleTrade { get; set; }
        private CurrencyTrade _removeGoldTrade { get; set; }
        private CurrencyTrade _addGoldTrade { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _peopleCurrency = CurrencyFactory.CreatePeople();
            _goldCurrency = CurrencyFactory.CreateGold();
            
            _addPeopleTrade = TestUtils.CreateTrade(AMOUNT, _peopleCurrency.CurrencyType, ActionType.ADD);            
            _removePeopleTrade = TestUtils.CreateTrade(AMOUNT, _peopleCurrency.CurrencyType, ActionType.REMOVE);
            _removeGoldTrade = TestUtils.CreateTrade(AMOUNT, _goldCurrency.CurrencyType, ActionType.REMOVE);
            _addGoldTrade = TestUtils.CreateTrade(AMOUNT, _goldCurrency.CurrencyType, ActionType.ADD);
        }

        [SetUp]
        public void SetUp()
        {
            _peopleCurrency = CurrencyFactory.CreatePeople();
            _goldCurrency = CurrencyFactory.CreateGold();
            
            _repository.Clear();
            _repository.Add(_peopleCurrency.CurrencyType, _peopleCurrency);
            _repository.Add(_goldCurrency.CurrencyType, _goldCurrency);
        }

        private void UpdateCurrencyTestRunner(bool success, params CurrencyTrade[] trades)
        {
            ServiceResponse response = _currencyController.UpdateCurrency(trades);

            if (success)
            {
                AssertSuccessResponse(response);
            }
            else
            {
                AssertFailureResponse(response);
            }
        }

        private static void AssertFailureResponse(ServiceResponse serviceResponse)
        {
            Assert.IsFalse(serviceResponse.IsSuccess);
            Assert.NotNull(serviceResponse.Message);
        }
        
        private static void AssertSuccessResponse(ServiceResponse serviceResponse)
        {
            Assert.IsTrue(serviceResponse.IsSuccess);
            Assert.IsNull(serviceResponse.Message);
        }

        private void AssertAmount(int expectedAmount, CurrencyType currency)
        {
            Currency repositoryCurrency = _repository.Get(currency);
            Assert.AreEqual(expectedAmount, repositoryCurrency.Amount);
        }

       [Test]
        public void Positive_UpdateCurrency_AddAmount_UpdatesCurrency()
        {
            UpdateCurrencyTestRunner( true, _addPeopleTrade);
            
            AssertAmount(AMOUNT, _peopleCurrency.CurrencyType);
        }

        [Test]
        public void Positive_UpdateCurrency_RemoveAmount_UpdatesCurrency()
        {
            UpdateCurrencyTestRunner( true, _removeGoldTrade, _addGoldTrade, _addGoldTrade);
            
            AssertAmount(AMOUNT, _goldCurrency.CurrencyType);
        }

        [Test]
        public void Positive_UpdateCurrency_MultipleTrades_UpdatesCurrency()
        {
            UpdateCurrencyTestRunner(true, _removeGoldTrade, _addGoldTrade, _addGoldTrade, _addPeopleTrade, _addPeopleTrade, _addPeopleTrade, _removePeopleTrade);
            
            AssertAmount(AMOUNT, _goldCurrency.CurrencyType);
            AssertAmount(20, _peopleCurrency.CurrencyType);
        }

        [Test]
        public void Negative_UpdateCurrency_ZeroAmount_ReturnsFalse()
        {
            UpdateCurrencyTestRunner(false, _removeGoldTrade);
            
            AssertAmount(0, _goldCurrency.CurrencyType);
        }

        [Test]
        public void Negative_UpdateCurrency_NoType_Throws()
        {
            CurrencyTrade currencyTrade = TestUtils.CreateTrade(AMOUNT, CurrencyType.NO_TYPE, ActionType.ADD);
            
            Assert.Throws<NoTypeException>(() =>
            {
                ServiceResponse serviceResponse = _currencyController.UpdateCurrency(currencyTrade);
                
                Assert.IsFalse(serviceResponse.IsSuccess);
                Assert.AreEqual(ErrorConstants.NO_TYPE_MESSAGE, serviceResponse.Message);
            });
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Negative_UpdateCurrency_BadAmount_ReturnsFalse(int amount)
        {
            CurrencyTrade currencyTrade = TestUtils.CreateTrade(amount, _goldCurrency.CurrencyType, ActionType.ADD);
            
            ServiceResponse serviceResponse = _currencyController.UpdateCurrency(currencyTrade);
            
            Assert.IsFalse(serviceResponse.IsSuccess);
            Assert.AreEqual(ErrorConstants.BAD_NUMBER_MESSAGE, serviceResponse.Message);
        }
    }
}