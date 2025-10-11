using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    /// <summary>
    /// This command is used to update a Currency model
    /// </summary>
    public readonly record struct CurrencyUpdate
    {
        /// <summary>
        /// What <see cref="Core.Contracts.Enum.CurrencyType"/> the action should perform on
        /// </summary>
        public required CurrencyType CurrencyType { get; init; }

        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="Core.Contracts.Enum.CurrencyType"/>
        /// </summary>
        /// <remarks>This number should be positive. We do not accept zero</remarks>
        public required uint Amount { get; init; }

        /// <summary>
        /// <inheritdoc cref="Core.Contracts.Enum.ActionType"/>
        /// </summary>
        public required ActionType ActionType { get; init; }
    }
}