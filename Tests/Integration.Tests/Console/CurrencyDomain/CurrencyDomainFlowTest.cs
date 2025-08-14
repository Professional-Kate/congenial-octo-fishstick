using IdelPog.Console;
using IdelPog.Console.Exceptions;
using IdelPog.Console.Runtime.Input;
using IdelPog.Console.Runtime.Input.Exceptions;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Integration.Tests.Console.Permission;

namespace IdelPog.Integration.Tests.Console
{
    [TestFixture]
    public class CurrencyDomainFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;

        private CurrencyUpdateListener _currencyUpdateListener;

        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
            TestPermissionService.SendAddPermissionCall(_inputHandler, Domain.CURRENCY);

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
                Assert.That(actualUpdate.ActionType, Is.EqualTo(expectedUpdate.ActionType));
                Assert.That(actualUpdate.Amount, Is.EqualTo(expectedUpdate.Amount));
                Assert.That(actualUpdate.CurrencyType, Is.EqualTo(expectedUpdate.CurrencyType));
            });
        }

        private CurrencyUpdate GetListenerCurrencyUpdate()
        {
            return _currencyUpdateListener.Buffer![0];
        }

        private static IEnumerable<TestCaseData> ValidCurrencyCases()
        {
            yield return new TestCaseData(
                new[] { "currency", "add", "10", "gold" },
                new CurrencyUpdate { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GOLD }
            ).SetName("Add_10_Gold");
            
            yield return new TestCaseData(
                new[] { "currency", "add", "0", "gold" },
                new CurrencyUpdate { ActionType = ActionType.ADD, Amount = 0, CurrencyType = CurrencyType.GOLD }
            ).SetName("Add_0_Gold");

            yield return new TestCaseData(
                new[] { "currency", "REMOVE", "0", "gold" },
                new CurrencyUpdate { ActionType = ActionType.REMOVE, Amount = 0, CurrencyType = CurrencyType.GOLD }
            ).SetName("Remove_0_Gold");

            yield return new TestCaseData(
                new[] { "currency", "remove", "1", "GOLD" },
                new CurrencyUpdate { ActionType = ActionType.REMOVE, Amount = 1, CurrencyType = CurrencyType.GOLD }
            ).SetName("Remove_1_Gold");
        }

        [TestCaseSource(nameof(ValidCurrencyCases))]
        public void Positive_ChangeCurrency_DispatchesUpdate(string[] arguments, CurrencyUpdate expectedUpdate)
        {
            _inputHandler.Input(new ReadOnlySpan<string>(arguments));
            AssertUpdateListener(true);
            AssertCurrencyUpdate(expectedUpdate, GetListenerCurrencyUpdate());
        }

        [TestCase(new[] { "currency", "remove", "-100", "gold" }, typeof(NegativeNumberException), TestName = "NegativeAmount_ThrowsNegativeNumberException")]
        [TestCase(new[] { "currency", "add", "-100", "gold" }, typeof(NegativeNumberException), TestName = "NegativeAmount_ThrowsNegativeNumberException")]
        [TestCase(new[] { "UNKNOWN", "REMOVE", "1", "GOLD" }, typeof(FailedEnumParseException), TestName = "UnknownDomain_ThrowsFailedEnumParse")]
        [TestCase(new[] { "currency", "remove", "100F", "GOLD" }, typeof(FailedTypeParseException), TestName = "InvalidAmount_Float__ThrowsFailedTypeParseException")]
        [TestCase(new[] { "currency", "remove", "10+10", "GOLD" }, typeof(FailedTypeParseException), TestName = "InvalidAmount_Expression_ThrowsFailedTypeParseException")]
        [TestCase(new[] { "CURRENCY", "ADD", "1232", "WOOD" }, typeof(FailedEnumParseException), TestName = "UnknownCurrency_ThrowsFailedEnumParse")]
        [TestCase(new[] { "CURRENCY", "UPDATE", "42", "GOLD" }, typeof(FailedEnumParseException), TestName = "UnknownAction_ThrowsFailedEnumParse")]
        [TestCase(new[] { "CURRENCY", "UPDATE" }, typeof(InvalidArgumentCountException), TestName = "MissingAmountAndCurrency_ThrowsInvalidArgumentCountException")]
        [TestCase(new[] { "CURRENCY", "UPDATE", "5" }, typeof(InvalidArgumentCountException), TestName = "MissingCurrency_ThrowsInvalidArgumentCountException")]
        [TestCase(new[] { "CURRENCY", "REMOVE", "23", "GOLD", "please" }, typeof(InvalidArgumentCountException), TestName = "AddedArgument_ThrowsInvalidArgumentCountException")]
        [TestCase(new string[] { }, typeof(EmptySpanException), TestName = "NoArguments_ThrowsEmptySpanException")]
        public void Negative_BadArguments_Throws(string[] arguments, Type exception)
        {
            Assert.Throws(exception, () => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            AssertUpdateListener(false);
        }

        [Test]
        public void Negative_PermissionDenied_NoUpdate_Throws()
        {
            TestPermissionService.SendRemovePermissionCall(_inputHandler, Domain.CURRENCY);
            Assert.Throws<DomainPermissionDeniedException>(() => _inputHandler.Input(new ReadOnlySpan<string>(["currency", "remove", "1", "GOLD"])));
        }
    }
}