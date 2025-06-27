using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Flows.Currency
{
    /// <summary>
    /// This structure is used to update a Currency model. 
    /// </summary>
    /// <remarks>
    /// Uses the internal <see cref="CurrencyType"/> to dictate what Currency to update, this update is decided by the <see cref="ActionType"/>
    /// </remarks>
    public readonly record struct CurrencyTrade
    {
        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="CurrencyType"/>
        /// </summary>
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