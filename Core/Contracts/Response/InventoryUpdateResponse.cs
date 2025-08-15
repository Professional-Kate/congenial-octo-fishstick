namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct InventoryUpdateResponse
    {
        public required InventoryUpdateEntry[] InventoryUpdateEntry { get; init; }
    }
}