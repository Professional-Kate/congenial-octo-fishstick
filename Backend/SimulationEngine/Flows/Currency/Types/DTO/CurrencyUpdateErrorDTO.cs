using IdelPog.Common.DTO;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyUpdateErrorDTO
    {
        public required CurrencyUpdateDTO[] CurrencyUpdates { get; init; }
        public required ErrorDTO ErrorDetails { get; init; }
    }
}