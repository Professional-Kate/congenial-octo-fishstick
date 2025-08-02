namespace IdelPog.SimulationEngine.Inventory
{
    public readonly record struct ItemInfo
    {
        public required ItemID ItemID { get; init; }
        public required uint BaseSellPrice { get; init; }
        public required uint Amount { get; init; }
    }
}