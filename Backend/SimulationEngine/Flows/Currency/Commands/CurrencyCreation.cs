namespace IdelPog.SimulationEngine.Currency.Commands
{
    public readonly record struct CurrencyCreation
    {
        public required CurrencyType Currency { get; init; }
        public required int StartingAmount { get; init; }
    }
}