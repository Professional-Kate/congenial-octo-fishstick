namespace IdelPog.SimulationEngine.Inventory
{
    /// <summary>
    /// Builds a new <see cref="Item"/>
    /// </summary>
    /// <seealso cref="SellPrice"/>
    /// <seealso cref="Amount"/>
    /// <seealso cref="Build"/>
    public interface IItemBuilder
    {
        public IItemBuilder SellPrice(uint sellPrice);

        public IItemBuilder Amount(uint amount);

        public Item Build();
    }
}