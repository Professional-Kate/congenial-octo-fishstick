using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Currency.Commands
{
    /// <summary>
    /// This command is used to update a Currency model
    /// </summary>
    public readonly record struct CurrencyUpdate
    {
        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="CurrencyType"/>
        /// </summary>
        /// <remarks>This number should be positive. We do not accept zero</remarks>
        public required int Amount { get; init; }

        /// <summary>
        /// What <see cref="CurrencyType"/> the action should perform on
        /// </summary>
        public required CurrencyType Currency { get; init; } 

        /// <summary>
        /// <inheritdoc cref="ActionType"/>
        /// </summary>
        public required ActionType Action { get; init; }
    }
}