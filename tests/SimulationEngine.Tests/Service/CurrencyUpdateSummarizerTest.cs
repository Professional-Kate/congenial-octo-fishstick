using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyUpdateSummarizerTest
    {
        private ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private Mock<ICurrencyUpdateFactory> _currencyUpdateFactoryMock;

        private CurrencyUpdate _addGoldUpdate;
        private CurrencyUpdate _removeGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            _currencyUpdateSummarizer = new CurrencyUpdateSummarizer(_currencyUpdateFactoryMock.Object, new NumberAssertion(new ThrowHandler()),
                new ObjectNullAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()));

            _addGoldUpdate = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD);
            _removeGoldUpdate = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.REMOVE);
        }

        [SetUp]
        public void SetUp()
        {
            _currencyUpdateFactoryMock.Reset();
            SetupFactoryMock();
        }

        private void SetupFactoryMock()
        {
            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(_addGoldUpdate.CurrencyType, _addGoldUpdate.Action, _addGoldUpdate.Amount))
                .Returns(_addGoldUpdate);

            _currencyUpdateFactoryMock.Setup(library =>
                    library.CreateCurrencyUpdate(_removeGoldUpdate.CurrencyType, _removeGoldUpdate.Action, _removeGoldUpdate.Amount))
                .Returns(_removeGoldUpdate);
        }

        [Test]
        public void Positive_GetSummary_OneUpdate_ReturnsOneUpdate()
        {
            CurrencyUpdate[] updates = _currencyUpdateSummarizer.GetSummary([_addGoldUpdate]);

            Assert.That(updates, Has.Length.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.That(updates[0].Action, Is.EqualTo(_addGoldUpdate.Action));
                Assert.That(updates[0].CurrencyType, Is.EqualTo(_addGoldUpdate.CurrencyType));
                Assert.That(updates[0].Amount, Is.EqualTo(_addGoldUpdate.Amount));
            });
        }

        private void GetSummary_GoldCurrency_TestRunner(IReadOnlyList<CurrencyUpdate> updates, int finalAmount, ActionType actionType)
        {
            CurrencyUpdate finalUpdate = new() { Action = actionType, Amount = finalAmount, CurrencyType = CurrencyType.GOLD };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(CurrencyType.GOLD, actionType, finalAmount))
                .Returns(finalUpdate);

            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary(updates);

            Assert.That(summarizedUpdates, Has.Length.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.That(summarizedUpdates[0].Action, Is.EqualTo(actionType));
                Assert.That(summarizedUpdates[0].CurrencyType, Is.EqualTo(CurrencyType.GOLD));
                Assert.That(summarizedUpdates[0].Amount, Is.EqualTo(finalAmount));
            });
        }

        [Test]
        public void Positive_GetSummary_MultipleGoldUpdates_ReturnsOneAddUpdate()
        {
            GetSummary_GoldCurrency_TestRunner([_addGoldUpdate, _addGoldUpdate, _addGoldUpdate], 30, ActionType.ADD);
        }

        [Test]
        public void Positive_GetSummary_MultipleGoldUpdates_ReturnsOneRemoveUpdate()
        {
            GetSummary_GoldCurrency_TestRunner([_removeGoldUpdate, _removeGoldUpdate], 20, ActionType.REMOVE);
        }

        [Test]
        public void Positive_GetSummary_CurrencyEndsWithZeroGold_NoReturn()
        {
            CurrencyUpdate[] updates = _currencyUpdateSummarizer.GetSummary([_addGoldUpdate, _removeGoldUpdate]);

            Assert.That(updates, Has.Length.EqualTo(0));
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<CurrencyType>(), It.IsAny<ActionType>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public void Positive_GetSummary_MultipleTypes_ReturnsSummaryForEach()
        {
            CurrencyUpdate addGemsUpdate = new() { Action = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(CurrencyType.GOLD, ActionType.REMOVE, 20))
                .Returns(new CurrencyUpdate { Action = ActionType.REMOVE, Amount = 20, CurrencyType = CurrencyType.GOLD });

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(CurrencyType.GEMS, ActionType.ADD, 10))
                .Returns(addGemsUpdate);

            CurrencyUpdate[] summarizedUpdates =
                _currencyUpdateSummarizer.GetSummary([_removeGoldUpdate, addGemsUpdate, _addGoldUpdate, _removeGoldUpdate, _removeGoldUpdate]);

            Assert.That(summarizedUpdates, Has.Length.EqualTo(2));

            Assert.Multiple(() =>
            {
                foreach (CurrencyUpdate summarizedUpdate in summarizedUpdates)
                {
                    switch (summarizedUpdate.CurrencyType)
                    {
                        case CurrencyType.GOLD:
                            Assert.That(summarizedUpdate.CurrencyType, Is.EqualTo(CurrencyType.GOLD));
                            Assert.That(summarizedUpdate.Amount, Is.EqualTo(20));
                            Assert.That(summarizedUpdate.Action, Is.EqualTo(ActionType.REMOVE));
                            break;
                        case CurrencyType.GEMS:
                            Assert.That(summarizedUpdate.CurrencyType, Is.EqualTo(CurrencyType.GEMS));
                            Assert.That(summarizedUpdate.Amount, Is.EqualTo(10));
                            Assert.That(summarizedUpdate.Action, Is.EqualTo(ActionType.ADD));
                            break;
                        default:
                            continue;
                    }
                }
            });
        }

        [Test]
        public void Positive_GetSummary_MultipleTypes_OneZeroAmount_ReturnsOneUpdate()
        {
            CurrencyUpdate addGemsUpdate = new() { Action = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(CurrencyType.GEMS, ActionType.ADD, 10))
                .Returns(addGemsUpdate);

            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary([_removeGoldUpdate, addGemsUpdate, _addGoldUpdate]);

            Assert.That(summarizedUpdates, Has.Length.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(summarizedUpdates[0].Action, Is.EqualTo(addGemsUpdate.Action));
                Assert.That(summarizedUpdates[0].CurrencyType, Is.EqualTo(addGemsUpdate.CurrencyType));
                Assert.That(summarizedUpdates[0].Amount, Is.EqualTo(addGemsUpdate.Amount));
            });

            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(CurrencyType.GOLD, It.IsAny<ActionType>(), It.IsAny<int>()), Times.Never);
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(CurrencyType.GEMS, It.IsAny<ActionType>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Negative_GetSummary_EmptyList_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateSummarizer.GetSummary([]));
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<CurrencyType>(), It.IsAny<ActionType>(), It.IsAny<int>()),
                Times.Never);

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
        }

        [Test]
        public void Negative_GetSummary_NullList_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateSummarizer.GetSummary(null!));
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<CurrencyType>(), It.IsAny<ActionType>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public void Negative_GetSummary_TradeContainsNegativeNumber_Throws()
        {
            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() =>
                _currencyUpdateSummarizer.GetSummary([
                    _addGoldUpdate, new CurrencyUpdate { Action = ActionType.ADD, Amount = -10, CurrencyType = CurrencyType.GOLD }
                ]));

            Assert.That(exception.Number, Is.EqualTo(-10));

            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<CurrencyType>(), It.IsAny<ActionType>(), It.IsAny<int>()),
                Times.Never);
        }
    }
}