using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Structures;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class CurrencyUpdateMediatorTest
    {
        private ICurrencyUpdateMediator _currencyUpdateMediator { get; set; }
        private Mock<IStateRepository<CurrencyType, Currency>> _repositoryMock { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Mock<IAssertPositive> _assertPositiveMock { get; set; }
        private Mock<IHandler>  _handlerMock { get; set; }
        private Mock<ICurrencyUpdateDispatcher>  _dispatcherMock { get; set; }
        private Mock<ICurrencyUpdateFactory> _currencyUpdateFactoryMock { get; set; }
        
        private Currency _goldCurrency { get; set; }
        private Currency _gemsCurrency { get; set; }

        private const int AMOUNT = 10;

        private static CurrencyTrade _addFoodTrade { get; set; }
        private static CurrencyTrade _removeFoodTrade { get; set; }
        private static CurrencyTrade _addWoodTrade { get; set; } 
        private static CurrencyTrade _removeWoodTrade { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CreateTrades();
        }
        
        [SetUp]
        public void Setup()
        {
            _goldCurrency = CurrencyFactory.CreateGold();
            _gemsCurrency = CurrencyFactory.CreateGems();
            
            SetupMock();
        }

        private void SetupMock()
        {
            _repositoryMock = new Mock<IStateRepository<CurrencyType, Currency>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _dispatcherMock = new Mock<ICurrencyUpdateDispatcher>();
            _assertPositiveMock = new Mock<IAssertPositive>();
            _handlerMock =  new Mock<IHandler>();
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            
            _currencyUpdateMediator = new CurrencyUpdateMediator(_currencyServiceMock.Object, _repositoryMock.Object, _dispatcherMock.Object, _assertPositiveMock.Object, new AssertCollectionNotEmpty(_handlerMock.Object));

            _repositoryMock.Setup(library => library.Get(CurrencyType.GOLD)).Returns(_goldCurrency.DeepClone());
            _repositoryMock.Setup(library => library.Get(CurrencyType.GEMS)).Returns(_gemsCurrency.DeepClone());
            
            _repositoryMock.Setup(library => library.Contains(It.IsAny<CurrencyType>())).Returns(true);

            _assertPositiveMock.Setup(library => library.AssertNumberIsPositive(It.IsAny<int[]>()));

            _currencyServiceMock.Setup(library => library.AddAmount(It.IsAny<Currency>(), It.IsAny<int>()))
                .Callback<Currency, int>((currency, amount) =>
                {
                    int newAmount = currency.Amount + amount;
                    currency.SetAmount(newAmount);
                });

            _currencyServiceMock.Setup(library => library.RemoveAmount(It.IsAny<Currency>(), It.IsAny<int>()))
                .Callback<Currency, int>((currency, amount) =>
                {
                    int newAmount = currency.Amount - amount;
                    currency.SetAmount(newAmount);
                });

            _repositoryMock.Setup(library => library.Update(It.IsAny<CurrencyType>(), It.IsAny<Currency>()))
                .Callback<CurrencyType, Currency>((type, currency) =>
                {
                    switch (type)
                    {
                        case CurrencyType.GOLD:
                            _goldCurrency = currency;
                            break;
                        case CurrencyType.GEMS:
                            _gemsCurrency = currency;
                            break;
                    }
                });
        }

        private static void CreateTrades()
        {
            _addFoodTrade = TestUtils.CreateTrade(AMOUNT, CurrencyType.GOLD, ActionType.ADD);
            _removeFoodTrade = TestUtils.CreateTrade(AMOUNT, CurrencyType.GOLD, ActionType.REMOVE);
            _addWoodTrade = TestUtils.CreateTrade(AMOUNT, CurrencyType.GEMS, ActionType.ADD);
            _removeWoodTrade = TestUtils.CreateTrade(AMOUNT, CurrencyType.GEMS, ActionType.REMOVE);
        }

        private void VerifyUpdateCall(int amount)
        {
            _repositoryMock.Verify(library => library.Update(It.IsAny<CurrencyType>(), It.IsAny<Currency>()), Times.Exactly(amount));
        }

        private void VerifyContainsCalls(int amount)
        {
            _repositoryMock.Verify(library => library.Contains(It.IsAny<CurrencyType>()), Times.Exactly(amount));
        }

        private void VerifyGetCalls(int amount)
        {
            _repositoryMock.Verify(library => library.Get(It.IsAny<CurrencyType>()), Times.Exactly(amount));
        }

        private void VerifyDispatcherCalls(int amount)
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<CurrencyTrade[]>()),  Times.Exactly(amount));
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] trades = Enumerable.Repeat(_addFoodTrade, tradeCount).ToArray();

            _currencyUpdateMediator.ProcessCurrencyUpdate(trades);
            
            Assert.That(tradeCount * AMOUNT, Is.EqualTo(_goldCurrency.Amount));

            VerifyContainsCalls(1);
            VerifyGetCalls(1);
            VerifyUpdateCall(1);
            VerifyDispatcherCalls(1);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] removeTrades = Enumerable.Repeat(_removeFoodTrade, tradeCount).ToArray();
            CurrencyTrade[] addTrades = Enumerable.Repeat(_addFoodTrade, tradeCount + 1).ToArray();
            
            _currencyUpdateMediator.ProcessCurrencyUpdate(addTrades);
            _currencyUpdateMediator.ProcessCurrencyUpdate(removeTrades);
            
            Assert.That(10, Is.EqualTo(_goldCurrency.Amount));

            VerifyContainsCalls(2);
            VerifyGetCalls(2);
            VerifyUpdateCall(2);
            VerifyDispatcherCalls(2);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_NoPassedTrades_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<CollectionEmptyException>()))
                .Throws(new CollectionEmptyException());
            
            CurrencyTrade[] trades = [];
            
            Assert.Throws<CollectionEmptyException>(() => _currencyUpdateMediator.ProcessCurrencyUpdate(trades));
            
            VerifyContainsCalls(0);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
            VerifyDispatcherCalls(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_CurrencyNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(CurrencyType.GOLD)).Returns(false);

            Assert.Throws<Exception>(() => _currencyUpdateMediator.ProcessCurrencyUpdate([_addFoodTrade]));
            
            Assert.That(0, Is.EqualTo(_goldCurrency.Amount));
            
            VerifyContainsCalls(1);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
            VerifyDispatcherCalls(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_OneCurrencyNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(CurrencyType.GEMS)).Returns(false);

            CurrencyTrade[] trades = { _addFoodTrade, _addWoodTrade };
            
            Assert.Throws<Exception>(() => _currencyUpdateMediator.ProcessCurrencyUpdate(trades));
            
            Assert.That(0, Is.EqualTo(_goldCurrency.Amount));
            Assert.That(0, Is.EqualTo(_gemsCurrency.Amount));
            
            VerifyContainsCalls(2);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
            VerifyDispatcherCalls(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_PassedTradesResultInNegativeAmount__Throws()
        {
            _assertPositiveMock.Setup(library => library.AssertNumberIsPositive(-10))
                .Throws(new NegativeNumberException(-1));
            
            CurrencyTrade[] trades = { _removeWoodTrade };
            
            Assert.Throws<NegativeNumberException>(() => _currencyUpdateMediator.ProcessCurrencyUpdate(trades));
            
            Assert.That(0, Is.EqualTo(_gemsCurrency.Amount));
            Assert.That(0, Is.EqualTo(_goldCurrency.Amount));
            VerifyDispatcherCalls(0);
        }
        
        [Test]
        public void Positive_ProcessCurrencyUpdate_MultipleTypeUpdates_UpdatesAmount()
        {
            // certain upgrades will cost multiple currency / give multiple currency for buying. This test is to prove it works.
            CurrencyTrade[] trades = { _removeFoodTrade, _removeWoodTrade, _addFoodTrade, _addWoodTrade, _addFoodTrade, _addWoodTrade };
            
            _currencyUpdateMediator.ProcessCurrencyUpdate(trades);
            
            Assert.That(10, Is.EqualTo(_gemsCurrency.Amount));
            Assert.That(10, Is.EqualTo(_goldCurrency.Amount)); 
            
            VerifyUpdateCall(2); // two currency = 2 update calls
            VerifyContainsCalls(2);
            VerifyGetCalls(2);
            VerifyDispatcherCalls(1);
        }
        
        [TestCase(-10, ActionType.ADD)]
        [TestCase(-100, ActionType.ADD)]
        [TestCase(-10, ActionType.REMOVE)]
        [TestCase(-100, ActionType.REMOVE)]
        public void Negative_ProcessCurrencyUpdate_BadAmounts_NoUpdates_Throws(int badAmount, ActionType action)
        {
            _assertPositiveMock.Setup(library => library.AssertNumberIsPositive(It.IsAny<int[]>()))
                .Throws(new NegativeNumberException(-1));
            
            CurrencyTrade trade = TestUtils.CreateTrade(badAmount, _goldCurrency.CurrencyType, action);

            Assert.Throws<NegativeNumberException>(() => _currencyUpdateMediator.ProcessCurrencyUpdate([trade]));
            
            Assert.That(0, Is.EqualTo(_goldCurrency.Amount));
            
            VerifyContainsCalls(0);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
            VerifyDispatcherCalls(0);
        }
        
        [Test]
        public void Negative_ProcessCurrencyUpdate_ArrayFails_NoUpdates_Throws()
        {
            _assertPositiveMock.Setup(library => library.AssertNumberIsPositive(It.IsAny<int[]>()))
                .Throws(new NegativeNumberException(-1));
            
            // First action is okay, 2nd action should stop processing for all actions 
            CurrencyTrade[] trades = { _addFoodTrade, _removeWoodTrade, _addFoodTrade };
            
            Assert.Throws<NegativeNumberException>(() => _currencyUpdateMediator.ProcessCurrencyUpdate(trades));
            
            Assert.That(0, Is.EqualTo(_goldCurrency.Amount));
            Assert.That(0, Is.EqualTo(_gemsCurrency.Amount));
            VerifyDispatcherCalls(0);
        }
    }
}