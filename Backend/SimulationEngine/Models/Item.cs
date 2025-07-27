using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Inventory;

namespace IdelPog.SimulationEngine.Models
{
    public sealed class Item : ICloneable<Item>
    {
        public readonly ItemID ItemID;
        public readonly uint BaseSellPrice;
        public readonly Information Information;
        public uint Amount { get; set; }
        
        public Item(ItemID itemID, uint baseSellPrice, Information information, uint amount)
        {
            ItemID = itemID;
            BaseSellPrice = baseSellPrice;
            Information = information;
            Amount = amount;
        }

        public Item DeepClone()
        {
            return new Item(ItemID, BaseSellPrice, Information, Amount);
        }
    }
}