namespace IdelPog.Service.Currency
{
    /// <summary>
    /// This class will handle modifying any data of the <see cref="Model.Currency"/> model. 
    /// </summary>
    /// <remarks>
    /// This class will not do any verification for any argument, the calling class is responsible for this behaviour.
    /// The responsibility of this class is to simply add or remove Amount from a <see cref="Model.Currency"/> model.
    /// </remarks>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public interface ICurrencyService
    {
        /// <summary>
        /// Adds the passed int amount to the passed <see cref="Model.Currency"/> model
        /// </summary>
        /// <param name="currency">The <see cref="Model.Currency"/> you want to add amount to</param>
        /// <param name="amount">The amount you want to add</param>
        /// <remarks>
        /// This method will do no verification of the passed arguments
        /// </remarks>
        public void AddAmount(Model.Currency currency, int amount);

        /// <summary>
        /// Removed the passed int amount from the passed <see cref="Model.Currency"/> model
        /// </summary>
        /// <param name="currency">The <see cref="Model.Currency"/> you want to remove amount from</param>
        /// <param name="amount">The amount you want to remove</param>
        /// <remarks>
        /// This method will do no verification of the passed arguments
        /// </remarks>
        public void RemoveAmount(Model.Currency currency, int amount);
    }
} 