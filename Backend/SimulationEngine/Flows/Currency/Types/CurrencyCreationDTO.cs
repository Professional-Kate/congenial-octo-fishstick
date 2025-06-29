namespace IdelPog.SimulationEngine.Currency
{
    public readonly record struct CurrencyCreationDTO
    {
        public required CurrencyType Currency { get; init; }
        public required int Amount { get; init; }
    }
}