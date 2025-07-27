namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct ItemDTO
    {
        public required ItemID ItemID { get; init; }
        public required uint SellPrice { get; init; }
        public required uint Amount { get; init; }
    }
}