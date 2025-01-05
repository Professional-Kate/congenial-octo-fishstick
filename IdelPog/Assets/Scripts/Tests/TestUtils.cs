using IdelPog.Repository.Currency;
using IdelPog.Structures;
using Moq;

namespace Tests
{
    /// <summary>
    /// Commonly used helper test methods. 
    /// </summary>
    /// <seealso cref="CreateTrade"/>
    /// <seealso cref="VerifyTotalGetCalls"/>
    internal static class TestUtils
    {
        /// <summary>
        /// Creates a <see cref="CurrencyTrade"/> object and returns it
        /// </summary>
        /// <param name="amount">The amount to modify</param>
        /// <param name="type">The <see cref="CurrencyType"/> you want to modify</param>
        /// <param name="action">The <see cref="ActionType"/></param>
        /// <returns></returns>
        internal static CurrencyTrade CreateTrade(int amount, CurrencyType type, ActionType action)
        {
            return new CurrencyTrade(amount, type, action);
        }
        
        /// <summary>
        /// Verify the total amount of <see cref="CurrencyRepository"/>.Get calls
        /// </summary>
        /// <param name="expected">The expected amount of calls</param>
        /// <param name="type">What <see cref="CurrencyType"/> was got</param>
        /// <param name="currencyRepositoryMock">The <see cref="CurrencyRepository"/> mock</param>
        internal static void VerifyTotalGetCalls(int expected, CurrencyType type, Mock<ICurrencyRepository> currencyRepositoryMock)
        {
            currencyRepositoryMock.Verify(library => library.Get(type), Times.Exactly(expected));
        }
    }
}