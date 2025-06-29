namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct CurrencyCreationDTO
    {
        public required CurrencyType Currency { get; init; }
        public required int Amount { get; init; }
    }
}