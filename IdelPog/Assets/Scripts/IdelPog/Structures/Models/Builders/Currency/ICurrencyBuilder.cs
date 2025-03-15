using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Structures.Builders
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

        public Currency Build();
    }
}