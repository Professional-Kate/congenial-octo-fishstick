using IdelPog.Controller;
using NUnit.Framework;

namespace Tests.Integration
{
    [TestFixture]
    public class CurrencyFlowTest
    {
        private readonly ICurrencyController _currencyController = CurrencyController.GetInstance();
    }
}