using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Models
{
    public sealed class Item : ICloneable<Item>
    {
        public readonly ItemID ItemID;
        public readonly uint BaseSellPrice;
        public readonly Information Information;
        public uint Amount { get; set; }
        
        public Item(ItemID itemID, uint amount, uint baseSellPrice, Information information)
        {
            ItemID = itemID;
            Amount = amount;
            BaseSellPrice = baseSellPrice;
            Information = information;
        }

        public Item DeepClone()
        {
            return new Item(ItemID, Amount,  BaseSellPrice, Information);
        }
    }
}