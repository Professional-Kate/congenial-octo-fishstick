using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;

namespace IdelPog.Engine.Utilities.Builders.Currency
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

        public Structures.Models.Currency Build();
    }
}