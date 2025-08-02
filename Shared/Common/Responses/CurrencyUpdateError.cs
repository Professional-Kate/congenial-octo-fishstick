using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}