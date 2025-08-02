using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Responses
{
    public readonly record struct CurrencyCreationResponse
    {
        public required CurrencyCreation[] CurrencyCreations { get; init; }
    }
}