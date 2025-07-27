using IdelPog.Common.DTO;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyCreationErrorDTO
    {
        public required CurrencyCreationDTO CurrencyCreation { get; init; }
        public required ErrorDTO ErrorDetails { get; init; }
    }
}