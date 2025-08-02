using IdelPog.Common.Commands;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyUpdateResponse
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
    }
}