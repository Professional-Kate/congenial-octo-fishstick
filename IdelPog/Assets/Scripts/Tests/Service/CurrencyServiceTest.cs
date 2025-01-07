using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository.Currency;
using IdelPog.Service;
using IdelPog.Structures;
using Moq;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture(CurrencyType.FOOD, 10)]
    public class CurrencyServiceTest
    {
        private TestableCurrencyService _currencyService { get; set; }
        private Mock<ICurrencyRepository> _currencyRepositoryMock { get; set; }
        private Currency _currency { get; set; }
        
        private readonly CurrencyType _currencyType;
        private readonly int _amount;

        public CurrencyServiceTest(CurrencyType currencyType, int amount)
        {
            _currencyType = CurrencyType.FOOD;
            _amount = amount;
        }
        
        [SetUp]
        public void Setup()
        {
            _currency = new Currency(_currencyType);
            SetupMock();
        }

        private void SetupMock()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _currencyService = new TestableCurrencyService(_currencyRepositoryMock.Object);

            _currencyRepositoryMock.Setup(library => library.Get(_currencyType)).Returns(_currency);
            _currencyRepositoryMock.Setup(library => library.Get(CurrencyType.WOOD)).Throws<NotFoundException>();
        }
        
        /// <summary>
        /// Verify the total amount of <see cref="CurrencyRepository"/>.Get calls
        /// </summary>
        /// <param name="expected">The expected amount of calls</param>
        /// <param name="type">What <see cref="CurrencyType"/> was got</param>
        /// <param name="currencyRepositoryMock">The <see cref="CurrencyRepository"/> mock</param>
        private static void VerifyTotalGetCalls(int expected, CurrencyType type, Mock<ICurrencyRepository> currencyRepositoryMock)
        {
            currencyRepositoryMock.Verify(library => library.Get(type), Times.Exactly(expected));
        }

        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            ServiceResponse serviceResponse = _currencyService.AddAmount(_currencyType, _amount);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(_amount, _currency.Amount);
            VerifyTotalGetCalls(1, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                _currencyService.AddAmount(_currencyType, _amount);
                Assert.AreEqual(_amount * i, _currency.Amount);
                VerifyTotalGetCalls(i, _currencyType, _currencyRepositoryMock);
            }
        }

        [TestCase(-10)]
        [TestCase(0)]
        public void Negative_AddAmount_BadAmount_ReturnsBadServiceResponse(int amount)
        {
            ServiceResponse serviceResponse = _currencyService.AddAmount(_currencyType, amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            
            Assert.AreNotEqual(_amount, _currency.Amount);
            VerifyTotalGetCalls(0, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Negative_AddAmount_UnknownCurrency_ReturnsBadServiceResponse()
        {
            ServiceResponse serviceResponse = _currencyService.AddAmount(CurrencyType.WOOD, _amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            VerifyTotalGetCalls(0, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmountFromCurrency()
        {
            _currency.SetAmount(_amount + 1); // Currency can't go negative, so we need this
            
            ServiceResponse serviceResponse = _currencyService.RemoveAmount(_currencyType, _amount);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(1, _currency.Amount);
            VerifyTotalGetCalls(1, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Positive_RemoveAmount_CallingMultipleTimes_RemovesAmountFromCurrency()
        {
            _currency.SetAmount(_amount);
            
            for (int i = 10; i > 1; i--)
            {
                Assert.AreEqual(i, _currency.Amount);
                _currencyService.RemoveAmount(_currencyType, 1);
            }
            
            _currencyRepositoryMock.Verify(library => library.Get(_currencyType), Times.Exactly(9));
        }

        [TestCase(-10)]
        [TestCase(0)]
        public void Negative_RemoveAmount_BadAmount_ReturnsBadServiceResponse(int amount)
        {
            ServiceResponse serviceResponse = _currencyService.RemoveAmount(_currencyType, amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            
            VerifyTotalGetCalls(0, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Negative_RemoveAmount_UnknownCurrency_ReturnsBadServiceResponse()
        {
            _currency.SetAmount(_amount);
            
            ServiceResponse serviceResponse = _currencyService.RemoveAmount(CurrencyType.WOOD, _amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            VerifyTotalGetCalls(0, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Negative_RemoveAmount_InsufficientAmount_ReturnsBadServiceResponse()
        {
            ServiceResponse serviceResponse = _currencyService.RemoveAmount(CurrencyType.FOOD, _amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            VerifyTotalGetCalls(1, _currencyType, _currencyRepositoryMock);
        }

        [Test]
        public void Negative_RemoveAmount_AmountExactlyZero_ReturnsBadServiceResponse()
        {
            _currency.SetAmount(_amount);
            
            ServiceResponse serviceResponse = _currencyService.RemoveAmount(_currencyType, _amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            VerifyTotalGetCalls(1, _currencyType, _currencyRepositoryMock);
        }
    }
}