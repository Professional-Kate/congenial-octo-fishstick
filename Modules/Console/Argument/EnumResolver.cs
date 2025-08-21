using IdelPog.Console.Argument.Interface;
using IdelPog.Console.Assertion.Interface;

namespace IdelPog.Console.Argument
{
    public class EnumResolver<TEnum> : IArgumentResolver<TEnum> where TEnum : struct
    {
        private readonly IEnumParseAssertion _assertCanParse;

        public EnumResolver(IEnumParseAssertion assertCanParse)
        {
            _assertCanParse = assertCanParse;
        }

        public TEnum Resolve(string argument)
        {
            bool successfulParse = Enum.TryParse(argument, true, out TEnum result);
            _assertCanParse.AssertCanParse(successfulParse, argument, nameof(TEnum));

            return result;
        }
    }
}