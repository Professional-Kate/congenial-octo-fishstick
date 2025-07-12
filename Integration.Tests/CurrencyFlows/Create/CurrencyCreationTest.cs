using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;

namespace Integration.Tests.CurrencyFlows.Create
{
    [TestFixture]
    public class CurrencyCreationTest : ManagedBuffer
    {
        private CurrencyCreationDTOListener _currencyCreationDTOListener;
        private CurrencyCreationErrorListener _currencyCreationErrorListener;

        private CurrencyCreation _createGold;
        private CurrencyCreation _createGems;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _createGold = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GOLD,
                StartingAmount = 10
            };

            _createGems = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GEMS,
                StartingAmount = 10
            };
        }

        [SetUp]
        public void SetUp()
        {
            _currencyCreationDTOListener = new CurrencyCreationDTOListener();
            _currencyCreationErrorListener = new CurrencyCreationErrorListener();
            
            ManagedSubscribe(_currencyCreationDTOListener);
            ManagedSubscribe(_currencyCreationErrorListener);
        }
    }
}