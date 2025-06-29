using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Currency
{
    public readonly record struct CurrencyUpdateDTO
    {
        public required int Amount { get; init; }

        public required CurrencyType Currency { get; init; } 

        public required ActionType Action { get; init; }
    }
}