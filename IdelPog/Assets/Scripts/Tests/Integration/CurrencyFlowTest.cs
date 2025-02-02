using IdelPog.Controller;
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

       [Test]
        public void Positive_UpdateCurrency_AddAmount_UpdatesCurrency()
        {
            _currencyController.UpdateCurrency(_addPeopleTrade);
            
            Currency repositoryCurrency = _repository.Get(_peopleCurrency.CurrencyType);
            Assert.AreEqual(AMOUNT, repositoryCurrency.Amount);
        }

        [Test]
        public void Positive_UpdateCurrency_RemoveAmount_UpdatesCurrency()
        {
            _currencyController.UpdateCurrency(_removeGoldTrade, _addGoldTrade, _addGoldTrade);
            
            Currency repositoryCurrency = _repository.Get(_goldCurrency.CurrencyType);
            Assert.AreEqual(10, repositoryCurrency.Amount);
        }

        [Test]
        public void Positive_UpdateCurrency_MultipleTrades_UpdatesCurrency()
        {
            _currencyController.UpdateCurrency(_removeGoldTrade, _addGoldTrade, _addGoldTrade, _addPeopleTrade, _addPeopleTrade, _addPeopleTrade, _removePeopleTrade);
            
            Currency repositoryGold = _repository.Get(_goldCurrency.CurrencyType);
            Currency repositoryPeople = _repository.Get(_peopleCurrency.CurrencyType);
            
            Assert.AreEqual(10, repositoryGold.Amount);
            Assert.AreEqual(20, repositoryPeople.Amount);
        }
    }
}