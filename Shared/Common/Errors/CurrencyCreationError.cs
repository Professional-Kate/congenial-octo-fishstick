using IdelPog.Common.Errors;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyCreationError
    {
        public required CurrencyCreation[] CurrencyCreations { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}