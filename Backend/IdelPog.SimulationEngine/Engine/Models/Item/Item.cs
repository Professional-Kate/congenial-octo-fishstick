using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Models
{
    /// <summary>
    /// The Item model
    /// </summary>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public class Item(InventoryID id, Information information, int sellPrice, int amount = 1)
        : ICloneable<Item>
    {
        public readonly InventoryID ID = id;
        public readonly Information Information = information;
        public readonly int SellPrice = sellPrice;
        
        public int Amount { get; private set; } = amount;

        public void AddAmount(int amount)
        {
            Amount += amount;
        }

        public void RemoveAmount(int amount)
        {
            Amount -= amount;
        }

        public Item DeepClone()
        {
            return new Item(ID, Information, SellPrice, Amount);
        }
    }
}