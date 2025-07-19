using Console;
using Console.Commands.Resolver.Exceptions;
using Console.Runtime.Input;
using Console.Runtime.Input.Exceptions;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Exceptions;

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

        private void AssertUpdateListener(bool wasCalled)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateListener.WasCalled, Is.EqualTo(wasCalled));

                if (wasCalled)
                {
                    Assert.That(_currencyUpdateListener.Buffer, Has.Count.EqualTo(1));
                }
            });
        }

        private void AssertCurrencyUpdate(CurrencyUpdate actualUpdate, CurrencyUpdate expectedUpdate)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actualUpdate.Action, Is.EqualTo(expectedUpdate.Action));
                Assert.That(actualUpdate.Amount, Is.EqualTo(expectedUpdate.Amount));
                Assert.That(actualUpdate.CurrencyType, Is.EqualTo(expectedUpdate.CurrencyType));
            });
        }

        private CurrencyUpdate GetListenerCurrencyUpdate()
        {
            return _currencyUpdateListener.Buffer[0];
        }

        [Test]
        public void Positive_AddCurrencyToGold_DispatchesUpdate()
        {
            string[] arguments =
            [
                "currency", "add", "10", "gold"
            ];
            
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            AssertUpdateListener(true);
            AssertCurrencyUpdate(new CurrencyUpdate { Action = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GOLD }, GetListenerCurrencyUpdate());
        }
        
        [Test]
        public void Positive_RemoveCurrencyFromGold_DispatchesUpdate()
        {
            string[] arguments =
            [
                "CURRENCY", "REMOVE", "0", "GOLD"
            ];
            
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            AssertUpdateListener(true);
            AssertCurrencyUpdate(new CurrencyUpdate { Action = ActionType.REMOVE, Amount = 0, CurrencyType = CurrencyType.GOLD }, GetListenerCurrencyUpdate());
        }
        
        [Test]
        public void Positive_AddNegativeAmount_DispatchesUpdate()
        {
            string[] arguments =
            [
                "CURRENCY", "add", "-100", "gold"
            ];
            
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            AssertUpdateListener(true);
            AssertCurrencyUpdate(new CurrencyUpdate { Action = ActionType.ADD, Amount = -100, CurrencyType = CurrencyType.GOLD }, GetListenerCurrencyUpdate());
        }
        
        [TestCase(new[] {"UNKNOWN", "REMOVE", "1", "GOLD"}, typeof(FailedEnumParseException), TestName = "UnknownDomain_ThrowsFailedEnumParse")]
        [TestCase(new[] {"CURRENCY", "ADD", "1232", "WOOD"}, typeof(FailedEnumParseException), TestName = "UnknownCurrency_ThrowsFailedEnumParse")]
        [TestCase(new[] {"CURRENCY", "UPDATE", "42", "GOLD"}, typeof(FailedEnumParseException), TestName = "UnknownAction_ThrowsFailedEnumParse")]
        [TestCase(new[] {"CURRENCY", "UPDATE"}, typeof(InvalidArgumentCountException), TestName = "MissingAmountAndCurrency_ThrowsInvalidArgumentCountException")]
        [TestCase(new[] {"CURRENCY", "UPDATE", "5"}, typeof(InvalidArgumentCountException), TestName = "MissingCurrency_ThrowsInvalidArgumentCountException")]
        [TestCase(new[] {"CURRENCY", "REMOVE", "23", "GOLD", "please"}, typeof(InvalidArgumentCountException), TestName = "AddedArgument_ThrowsInvalidArgumentCountException")]
        [TestCase(new string[] {}, typeof(EmptySpanException), TestName = "NoArguments_ThrowsEmptySpanException")]
        public void Negative_BadArguments_Throws(string[] arguments, Type exception)
        {
            Assert.Throws(exception, () => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            AssertUpdateListener(false);
        }
    }
}