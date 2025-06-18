using IdelPog.SimulationEngine.Flows.Currency;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyUpdateFactoryTest
    {
        private ICurrencyUpdateFactory _currencyUpdateFactory { get; set; }
        private CurrencyTrade _currencyTrade { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _currencyUpdateFactory = new CurrencyUpdateFactory();
            _currencyTrade = TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD);
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            CurrencyUpdateDTO updateDTO =_currencyUpdateFactory.CreateFrom(_currencyTrade);
            
            Assert.Multiple(() =>
            {
                Assert.That(updateDTO.Action,  Is.EqualTo(ActionType.ADD));
                Assert.That(updateDTO.Amount,  Is.EqualTo(10));
                Assert.That(updateDTO.Currency,  Is.EqualTo(CurrencyType.FOOD));
            });
        }
    }
}