using IdelPog.Common.DTO;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyUpdateErrorDTO
    {
        public required CurrencyUpdateDTO CurrencyUpdate { get; init; }
        public required ErrorDTO ErrorDetails { get; init; }
    }
}