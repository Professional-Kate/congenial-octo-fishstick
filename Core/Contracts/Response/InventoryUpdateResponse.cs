namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct InventoryUpdateResponse
    {
        public required InventoryUpdateEntry[] InventoryUpdateEntries { get; init; }
    }
}