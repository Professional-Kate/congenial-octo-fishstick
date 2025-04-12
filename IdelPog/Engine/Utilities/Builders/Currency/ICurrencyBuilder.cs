using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;

namespace IdelPog.Engine.Utilities.Builders
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