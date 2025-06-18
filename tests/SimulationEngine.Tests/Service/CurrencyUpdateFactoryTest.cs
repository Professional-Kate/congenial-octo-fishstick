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
                TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.REMOVE),
                TestUtils.CreateTrade(int.MaxValue, CurrencyType.FOOD, ActionType.REMOVE),
                // The factory doesn't care about negatives. It should be verified elsewhere if negative numbers are an issue
                TestUtils.CreateTrade(-10, CurrencyType.FOOD, ActionType.ADD),
                TestUtils.CreateTrade(-10, CurrencyType.FOOD, ActionType.REMOVE),
                TestUtils.CreateTrade(int.MinValue, CurrencyType.FOOD, ActionType.REMOVE)
            ];
        }

        private void AssertCollection(IReadOnlyList<CurrencyUpdateDTO> currencyUpdateDTOs, IReadOnlyList<CurrencyTrade> currencyTrades)
        {
            for (int i = 0; i < currencyUpdateDTOs.Count; i++)
            {
                CurrencyUpdateDTO currencyUpdateDTO = currencyUpdateDTOs[i];
                CurrencyTrade currencyTrade = currencyTrades[i];
                
                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdateDTO.Action, Is.EqualTo(currencyTrade.Action));
                    Assert.That(currencyUpdateDTO.Amount, Is.EqualTo(currencyTrade.Amount));
                    Assert.That(currencyUpdateDTO.Currency, Is.EqualTo(currencyTrade.Currency));
                });
            }
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs =_currencyUpdateFactory.CreateFrom(_currencyTrades);
            
            Assert.That(updateDTOs, Has.Count.EqualTo(_currencyTrades.Count));
            
            AssertCollection(updateDTOs, _currencyTrades);
        }
    }
}