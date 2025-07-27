using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyCreationDTO
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
    }
}