using IdelPog.Model;

namespace IdelPog.Service
{
    /// <summary>
    /// This class will handle modifying any data of the <see cref="Currency"/> model. 
    /// </summary>
    /// <remarks>
    /// This class will not do any verification for any argument, the calling class is responsible for this behaviour.
    /// The responsibility of this class is to simply add or remove Amount from a <see cref="Currency"/> model.
    /// </remarks>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public interface ICurrencyService
    {
        /// <summary>
        /// Adds the passed int amount to the passed <see cref="Currency"/> model
        /// </summary>
        /// <param name="currency">The <see cref="Currency"/> you want to add amount to</param>
        /// <param name="amount">The amount you want to add</param>
        /// <remarks>
        /// This method will do no verification of the passed arguments
        /// </remarks>
        public void AddAmount(Currency currency, int amount);

        /// <summary>
        /// Removed the passed int amount from the passed <see cref="Currency"/> model
        /// </summary>
        /// <param name="currency">The <see cref="Currency"/> you want to remove amount from</param>
        /// <param name="amount">The amount you want to remove</param>
        /// <remarks>
        /// This method will do no verification of the passed arguments
        /// </remarks>
        public void RemoveAmount(Currency currency, int amount);
    }
} 