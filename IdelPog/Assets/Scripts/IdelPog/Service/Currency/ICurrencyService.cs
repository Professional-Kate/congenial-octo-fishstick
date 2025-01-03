using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    /// <summary>
    /// This class will handle modifying any data of the <see cref="Model.Currency"/> model.
    /// This class should only ever return a <see cref="ServiceResponse"/>, catch errors and wrap them in the <see cref="ServiceResponse"/> object so the controller doesn't need to 
    /// </summary>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public interface ICurrencyService
    {
        /// <summary>
        /// Adds a passed amount to a <see cref="Model.Currency"/> model by using its linked <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="currencyType">
        /// The <see cref="Model.Currency"/> you want to add an amount to.
        /// The <see cref="Model.Currency"/> will contain this passed <see cref="CurrencyType"/>
        /// </param>
        /// <param name="amount">The amount you want to add. This must be a positive number and not zero</param>
        /// <returns>
        /// A <see cref="ServiceResponse"/> that contains a boolean <see cref="ServiceResponse.IsSuccess"/> on the state of the operation.
        /// </returns>
        /// <remarks>
        /// Every implementation of this method should follow these rules :
        /// <list type="bullet">
        /// <item> You should return a failed <see cref="ServiceResponse"/> if the passed amount is not equal to or greater than zero</item>
        /// <item> You should also return a failed <see cref="ServiceResponse"/> if the passed <see cref="CurrencyType"/> is not found in the Repository </item>
        /// <item> Return a failed <see cref="ServiceResponse"/> if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/> </item>
        /// </list>
        /// </remarks>
        public ServiceResponse AddAmount(CurrencyType currencyType, int amount);
        
        /// <summary>
        /// Removes a passed amount from a <see cref="Model.Currency"/> model by using its linked <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="currencyType">
        /// The <see cref="Model.Currency"/> you want to remove amount from.
        /// The <see cref="Model.Currency"/> will contain this passed <see cref="CurrencyType"/>
        /// </param>
        /// <param name="amount">The amount you want to remove. This must be a positive number and not zero</param>
        /// <returns>
        /// A <see cref="ServiceResponse"/> that contains a boolean <see cref="ServiceResponse.IsSuccess"/> on the state of the operation.
        /// </returns>
        /// <remarks>
        /// Every implementation of this method should follow these rules :
        /// <list type="bullet">
        /// <item> You should return a failed <see cref="ServiceResponse"/> if the passed amount if not equal to or greater than zero</item>
        /// <item> You should also return a failed <see cref="ServiceResponse"/> if the passed <see cref="CurrencyType"/> is not found in the Repository </item>
        /// <item> Return a failed <see cref="ServiceResponse"/> if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/> </item>
        /// <item> Return a failed <see cref="ServiceResponse"/> if the passed amount would cause the <see cref="Model.Currency"/> to have a 0 or negative amount </item>
        /// </list>
        /// </remarks>
        public ServiceResponse RemoveAmount(CurrencyType currencyType, int amount);
    }
} 