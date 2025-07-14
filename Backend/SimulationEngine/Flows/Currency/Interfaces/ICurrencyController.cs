using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// All commands for mutating <see cref="Currency"/> will be defined here, each command will be atomic
    /// </summary>
    public interface ICurrencyController
    {
        /// <summary>
        /// Updates <see cref="Currency"/> in the Repository based on each <see cref="CurrencyUpdate"/> in the collection
        /// </summary>
        /// <param name="trades">A collection of valid <see cref="CurrencyUpdate"/> commands</param>
        /// <exception cref="NegativeNumberException">Will be thrown if any trades amount is not positive (zero or negative). Also thrown if committing the collection will leave any <see cref="Currency"/> negative</exception>
        /// <exception cref="CollectionEmptyException">Will be thrown if the collection has zero elements</exception>
        /// <exception cref="ArgumentNullException">Will be thrown if the collection is null</exception>
        /// <remarks>The collection is simulated first only being committed into the Repository if each command passes all validation</remarks>
        public void UpdateCurrency(IReadOnlyList<CurrencyUpdate> trades);
        
        /// <summary>
        /// Creates a new <see cref="CurrencyType"/>:<see cref="Currency"/> pair in the Repository
        /// </summary>
        /// <param name="commands">A collection of valid <see cref="CurrencyCreation"/> commands</param>
        /// <exception cref="DuplicateItemException">Will be thrown if any <see cref="CurrencyType"/> already exists in the Repository</exception>
        /// <exception cref="CollectionEmptyException">Will be thrown if the collection has zero elements</exception>
        /// <exception cref="ArgumentNullException">Will be thrown if the collection is null</exception>
        /// <remarks>The collection is simulated first only being committed into the Repository if each command passes all validation</remarks>
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands);
    }
}