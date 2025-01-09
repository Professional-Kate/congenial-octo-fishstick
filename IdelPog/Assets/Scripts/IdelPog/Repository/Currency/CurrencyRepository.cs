using IdelPog.Exceptions;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository.Currency
{
    /// <summary>
    /// This class provides access to Remove, Add, or Get a <see cref="Model.Currency"/> object. Please see <see cref="ICurrencyRepository"/> for documentation.
    /// </summary>
    public class CurrencyRepository : Repository<CurrencyType, Model.Currency>, ICurrencyRepository
    {
        public override void Add(CurrencyType key, Model.Currency value)
        {
            AssertTypeIsValid(key);
            
            base.Add(key, value);;
        }

        public override void Remove(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            
            base.Remove(currencyType);
        }

        public override Model.Currency Get(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            
            return base.Get(currencyType);
        }
        
        /// <summary>
        /// Asserts that the passed <see cref="CurrencyType"/> is not <see cref="CurrencyType.NO_TYPE"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="CurrencyType"/> you want to check</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        private static void AssertTypeIsValid(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.NO_TYPE)
            {
                throw new NoTypeException("Error! Passed CurrencyType is NO_TYPE, nothing can be retrieved. This should be fixed.");
            }
        }

       
    }
}
