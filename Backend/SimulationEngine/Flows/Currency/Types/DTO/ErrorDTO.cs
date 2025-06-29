namespace IdelPog.SimulationEngine.Currency.DTO
{
    public readonly record struct ErrorDTO
    {
        public required string ErrorMessage { get; init; }
        public required Exception Exception { get; init; }
    }
}