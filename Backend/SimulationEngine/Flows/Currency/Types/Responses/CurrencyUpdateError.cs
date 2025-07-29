using IdelPog.Common.Errors;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdateResponse[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}