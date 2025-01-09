namespace IdelPog.Structures.Enums
{
    /// <summary>
    /// Every Currency in the game will require one of these tags. Instead of passing around Currency objects we simply need to pass around these tags
    /// </summary>
    public enum CurrencyType : byte
    {
        NO_TYPE,
        FOOD,
        WOOD
    }
}