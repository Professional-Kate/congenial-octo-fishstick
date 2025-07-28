using IdelPog.Common.DTO.Error;

namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyCreationErrorDTO
    {
        public required CurrencyCreationDTO[] CurrencyCreations { get; init; }
        public required ErrorDTO ErrorDetails { get; init; }
    }
}