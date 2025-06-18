using IdelPog.SimulationEngine.Flows.Currency;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyUpdateFactoryTest
    {
        private ICurrencyUpdateFactory _currencyUpdateFactory { get; set; }
        private IReadOnlyList<CurrencyTrade> _currencyTrades { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _currencyUpdateFactory = new CurrencyUpdateFactory();
            _currencyTrades =
            [
                
                TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD),
                TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD),
                TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD),
                TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD)
            ];
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs =_currencyUpdateFactory.CreateFrom(_currencyTrades);
            
            Assert.That(updateDTOs, Has.Count.EqualTo(_currencyTrades.Count));
            
            foreach (CurrencyUpdateDTO currencyUpdateDTO in updateDTOs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdateDTO.Action,  Is.EqualTo(ActionType.ADD));
                    Assert.That(currencyUpdateDTO.Amount,  Is.EqualTo(10));
                    Assert.That(currencyUpdateDTO.Currency,  Is.EqualTo(CurrencyType.FOOD));
                });
            }
        }
    }
}