using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <summary>
    /// This class provides CRUD access to <see cref="Currency"/> models
    /// </summary>
    /// <seealso cref="ICurrencyRepositoryRead"/>
    /// <seealso cref="ICurrencyRepositoryWrite"/>
    /// <seealso cref="ICurrencyRepositoryUpdate"/>
    public class CurrencyRepository : Repository<CurrencyType, Currency>, ICurrencyRepositoryWrite, ICurrencyRepositoryRead, ICurrencyRepositoryUpdate
    {
        /// <inheritdoc cref="ICurrencyRepositoryWrite.Add"/>
        public override void Add(CurrencyType key, Currency value)
        {
            AssertTypeIsValid(key);

            base.Add(key, value);
        }

        /// <inheritdoc cref="ICurrencyRepositoryWrite.Remove"/>
        public override void Remove(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            
            base.Remove(currencyType);
        }

        /// <inheritdoc cref="ICurrencyRepositoryRead.Get"/>
        public override Currency Get(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            
            Currency foundCurrency = base.Get(currencyType);

            return foundCurrency.Clone() as Currency;
        }

        /// <inheritdoc cref="ICurrencyRepositoryUpdate.Update"/>
        public override void Update(CurrencyType key, Currency value)
        {
            AssertTypeIsValid(key);
            
            base.Update(key, value);
        }

        /// <summary>
        /// Asserts that the passed <see cref="CurrencyType"/> is not <see cref="CurrencyType.NO_TYPE"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="CurrencyType"/> you want to check</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <remarks>
        /// This exception is expected to not be caught. A <see cref="Currency"/> having the type <see cref="CurrencyType.NO_TYPE"/> is a data setup issue
        /// </remarks>
        private static void AssertTypeIsValid(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.NO_TYPE)
            {
                throw new NoTypeException("Error! Passed CurrencyType is NO_TYPE, nothing can be retrieved. This should be fixed.");
            }
        }

       
    }
}
