using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyCreationResponse
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
    }
}