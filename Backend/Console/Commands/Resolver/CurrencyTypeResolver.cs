using Console.Commands.Assertions;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver
{
    public class CurrencyTypeResolver : IArgumentResolver<CurrencyType>
    {
        private readonly IAssertCanParseEnum _assertCanParse;

        public CurrencyTypeResolver(IAssertCanParseEnum assertCanParse)
        {
            _assertCanParse = assertCanParse;
        }

        public CurrencyType Resolve(string argument)
        {
            bool successfulParse = Enum.TryParse(argument, ignoreCase: true, out CurrencyType result);
            _assertCanParse.Handle(successfulParse, argument, nameof(CurrencyType));

            return result;
        }
    }
}