namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct ItemDTO
    {
        public required ItemID ItemID { get; init; }
        public required int SellPrice { get; init; }
        public required int Amount { get; init; }
    }
}