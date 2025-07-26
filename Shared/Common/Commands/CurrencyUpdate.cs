using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    /// <summary>
    /// This command is used to update a Currency model
    /// </summary>
    public readonly record struct CurrencyUpdate
    {
        /// <summary>
        /// What <see cref="Common.Enums.CurrencyType"/> the action should perform on
        /// </summary>
        public required CurrencyType CurrencyType { get; init; }

        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="Common.Enums.CurrencyType"/>
        /// </summary>
        /// <remarks>This number should be positive. We do not accept zero</remarks>
        public required int Amount { get; init; }

        /// <summary>
        /// <inheritdoc cref="ActionType"/>
        /// </summary>
        public required ActionType Action { get; init; }
    }
}