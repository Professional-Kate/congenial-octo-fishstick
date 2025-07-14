using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Commands
{
    public readonly record struct CurrencyCreation
    {
        public required CurrencyType CurrencyType { get; init; }
        public required int StartingAmount { get; init; }
    }
}