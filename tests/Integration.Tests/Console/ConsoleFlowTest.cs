using Console;
using Console.Runtime.Input;
using IdelPog.Common.Enums;

namespace Integration.Tests.Console
{
    [TestFixture]
    public class ConsoleFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;
        
        private CurrencyUpdateListener _currencyUpdateListener;

        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
            
            _currencyUpdateListener = new CurrencyUpdateListener();
            ManagedSubscribe(_currencyUpdateListener);
        }

        [Test]
        public void Positive_CurrencyDomain_AddCurrencyToGold_DispatchesUpdate()
        {
            string[] arguments =
            [
                "currency", "add", "10", "gold"
            ];
            
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateListener.WasCalled, Is.True);
                Assert.That(_currencyUpdateListener.Buffer, Has.Count.EqualTo(1));

                CurrencyUpdate currencyUpdate = _currencyUpdateListener.Buffer[0];
                Assert.That(currencyUpdate.Action, Is.EqualTo(ActionType.ADD));
                Assert.That(currencyUpdate.Amount, Is.EqualTo(10));
                Assert.That(currencyUpdate.CurrencyType, Is.EqualTo(CurrencyType.GOLD));
            });
        }
    }
}