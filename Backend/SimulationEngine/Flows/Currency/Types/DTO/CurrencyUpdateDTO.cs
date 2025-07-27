using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyUpdateDTO
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
        public required ActionType Action { get; init; }
    }
}