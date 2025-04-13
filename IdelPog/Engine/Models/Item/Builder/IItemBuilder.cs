namespace IdelPog.Engine.Models
{
    /// <summary>
    /// Builds a new <see cref="Item"/>
    /// </summary>
    /// <seealso cref="SellPrice"/>
    /// <seealso cref="Amount"/>
    /// <seealso cref="Build"/>
    public interface IItemBuilder
    {
        public IItemBuilder SellPrice(int sellPrice);

        public IItemBuilder Amount(int amount);

        public Item Build();
    }
}