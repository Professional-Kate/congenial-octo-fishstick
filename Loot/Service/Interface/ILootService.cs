namespace IdelPog.Loot.Service.Interface
{
    public interface ILootService<in TID> where TID : Enum
    { 
        public void DispatchInventoryUpdates(TID id);
    }
}