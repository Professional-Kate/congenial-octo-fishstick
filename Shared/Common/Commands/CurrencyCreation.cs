using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct CurrencyCreation
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint StartingAmount { get; init; }
    }
}