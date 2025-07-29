using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyUpdateResponse
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
        public required ActionType Action { get; init; }
    }
}