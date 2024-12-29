using IdelPog.Repository;
using Moq;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class CurrencyServiceTest
    {
        private TestableCurrencyService _currencyService { get; set; }
        private Mock<ICurrencyRepository> _currencyRepositoryMock { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _currencyService = new TestableCurrencyService(_currencyRepositoryMock.Object);
        }
    }
}