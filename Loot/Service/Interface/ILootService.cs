using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Service.Interface
{
    public interface ILootService<in TID> where TID : Enum
    {
        public bool ShouldGrant(TID id);
        
        public ItemID GenerateItemID(TID id);
    }
}