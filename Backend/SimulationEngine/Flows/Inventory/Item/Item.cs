using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Inventory
{
    public class Item(ItemID id, Information information, uint sellPrice, uint amount = 1)
        : ICloneable<Item>
    {
        public readonly ItemID ID = id;
        public readonly Information Information = information;
        public readonly uint SellPrice = sellPrice;

        public uint Amount { get; private set; } = amount;

        public void AddAmount(uint amount)
        {
            Amount += amount;
        }

        public void RemoveAmount(uint amount)
        {
            Amount -= amount;
        }

        public Item DeepClone()
        {
            return new Item(ID, Information, SellPrice, Amount);
        }
    }
}