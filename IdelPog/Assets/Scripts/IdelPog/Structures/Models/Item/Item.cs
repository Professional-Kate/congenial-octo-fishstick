using System;

namespace IdelPog.Structures.Models.Item
{
    /// <summary>
    /// The Item model
    /// </summary>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public class Item : ICloneable
    {
        public readonly InventoryID ID;
        public readonly Information Information;
        public readonly int SellPrice;
        
        public int Amount { get; private set; }

        public Item(InventoryID id, Information information, int sellPrice, int amount = 1)
        {
            ID = id;
            Information = information;
            SellPrice = sellPrice;
            Amount = amount;
        }

        public void AddAmount(int amount)
        {
            Amount += amount;
        }

        public void RemoveAmount(int amount)
        {
            Amount -= amount;
        }

        public object Clone()
        {
            return new Item(ID, Information, SellPrice, Amount);
        }
    }
}