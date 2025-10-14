using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service;
using IdelPog.Currency.Service.Interface;
using Moq;

namespace IdelPog.Currency.Tests.Service
{
    [TestFixture]
    public sealed class CurrencyUpdateSummarizerTest
    {
        private ICurrencyUpdateSummarizer _currencyUpdateSummarizer;
        private Mock<ICurrencyUpdateFactory> _currencyUpdateFactoryMock;

        private CurrencyUpdate _addGoldUpdate;
        private CurrencyUpdate _removeGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            _currencyUpdateSummarizer = new CurrencyUpdateSummarizer(_currencyUpdateFactoryMock.Object, new CollectionAssertion());

            _addGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.ADD);
            _removeGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.REMOVE);
        }

        [SetUp]
        public void SetUp()
        {
            _currencyUpdateFactoryMock.Reset();
            SetupFactoryMock();
        }

        private void SetupFactoryMock()
        {
            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(_addGoldUpdate.ActionType, _addGoldUpdate.Amount, _addGoldUpdate.CurrencyType))
                .Returns(_addGoldUpdate);

            _currencyUpdateFactoryMock.Setup(library =>
                    library.CreateCurrencyUpdate(_removeGoldUpdate.ActionType, _removeGoldUpdate.Amount, _removeGoldUpdate.CurrencyType))
                .Returns(_removeGoldUpdate);
        }

        [Test]
        public void Positive_GetSummary_OneUpdate_ReturnsOneUpdate()
        {
            CurrencyUpdate[] updates = _currencyUpdateSummarizer.GetSummary([_addGoldUpdate]);

            Assert.That(updates, Has.Length.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.That(updates[0].ActionType, Is.EqualTo(_addGoldUpdate.ActionType));
                Assert.That(updates[0].CurrencyType, Is.EqualTo(_addGoldUpdate.CurrencyType));
                Assert.That(updates[0].Amount, Is.EqualTo(_addGoldUpdate.Amount));
            });
        }

        private void GetSummary_GoldCurrency_TestRunner(IReadOnlyList<CurrencyUpdate> updates, uint finalAmount, ActionType actionType)
        {
            CurrencyUpdate finalUpdate = new() { ActionType = actionType, Amount = finalAmount, CurrencyType = CurrencyType.GOLD };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(actionType, finalAmount, CurrencyType.GOLD))
                .Returns(finalUpdate);

            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary(updates);

            Assert.That(summarizedUpdates, Has.Length.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.That(summarizedUpdates[0].ActionType, Is.EqualTo(actionType));
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
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate( It.IsAny<ActionType>(), It.IsAny<uint>(), It.IsAny<CurrencyType>()),
                Times.Never);
        }

        [Test]
        public void Positive_GetSummary_MultipleTypes_ReturnsSummaryForEach()
        {
            CurrencyUpdate addGemsUpdate = new() { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(ActionType.REMOVE, 20, CurrencyType.GOLD))
                .Returns(new CurrencyUpdate { ActionType = ActionType.REMOVE, Amount = 20, CurrencyType = CurrencyType.GOLD });

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(ActionType.ADD, 10, CurrencyType.GEMS))
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
                            Assert.That(summarizedUpdate.ActionType, Is.EqualTo(ActionType.REMOVE));
                            break;
                        case CurrencyType.GEMS:
                            Assert.That(summarizedUpdate.CurrencyType, Is.EqualTo(CurrencyType.GEMS));
                            Assert.That(summarizedUpdate.Amount, Is.EqualTo(10));
                            Assert.That(summarizedUpdate.ActionType, Is.EqualTo(ActionType.ADD));
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
            CurrencyUpdate addGemsUpdate = new() { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };

            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(ActionType.ADD, 10, CurrencyType.GEMS))
                .Returns(addGemsUpdate);

            CurrencyUpdate[] summarizedUpdates = _currencyUpdateSummarizer.GetSummary([_removeGoldUpdate, addGemsUpdate, _addGoldUpdate]);

            Assert.That(summarizedUpdates, Has.Length.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(summarizedUpdates[0].ActionType, Is.EqualTo(addGemsUpdate.ActionType));
                Assert.That(summarizedUpdates[0].CurrencyType, Is.EqualTo(addGemsUpdate.CurrencyType));
                Assert.That(summarizedUpdates[0].Amount, Is.EqualTo(addGemsUpdate.Amount));
            });

            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<ActionType>(), It.IsAny<uint>(), CurrencyType.GOLD), Times.Never);
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<ActionType>(), It.IsAny<uint>(), CurrencyType.GEMS), Times.Once);
        }

        [Test]
        public void Negative_GetSummary_EmptyList_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateSummarizer.GetSummary([]));
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<ActionType>(), It.IsAny<uint>(), It.IsAny<CurrencyType>()),
                Times.Never);

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
        }

        [Test]
        public void Negative_GetSummary_NullList_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateSummarizer.GetSummary(null!));
            _currencyUpdateFactoryMock.Verify(library => library.CreateCurrencyUpdate(It.IsAny<ActionType>(), It.IsAny<uint>(), It.IsAny<CurrencyType>()),
                Times.Never);
        }
    }
}