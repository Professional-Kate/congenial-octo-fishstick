using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts.Command
{
    public readonly record struct CurrencyCreation
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint StartingAmount { get; init; }
    }
}