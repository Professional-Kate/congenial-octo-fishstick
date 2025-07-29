using IdelPog.Common.Errors;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyCreationError
    {
        public required CurrencyCreationResponse[] CurrencyCreations { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}