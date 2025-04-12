using IdelPog.Main.Structures.Enums;

namespace IdelPog.Main.Structures.Models.Builders.Currency
{
    /// <summary>
    /// Builds a new <see cref="Currency"/>
    /// </summary>
    /// <seealso cref="CurrencyType"/>
    /// <seealso cref="Amount"/>
    /// <seealso cref="Build"/>
    public interface ICurrencyBuilder
    {
        public ICurrencyBuilder CurrencyType(CurrencyType currencyType);

        public ICurrencyBuilder Amount(int amount);

        public Models.Currency Build();
    }
}