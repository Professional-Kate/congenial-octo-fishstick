using Console.Commands.Assertions;

namespace Console.Commands.Resolver
{
    public class EnumResolver<TEnum> : IArgumentResolver<TEnum> where TEnum : struct
    {
        private readonly IAssertCanParseEnum _assertCanParse;

        public EnumResolver(IAssertCanParseEnum assertCanParse)
        {
            _assertCanParse = assertCanParse;
        }
        
        public TEnum Resolve(string argument)
        {
            bool successfulParse = Enum.TryParse(argument, ignoreCase: true, out TEnum result);
            _assertCanParse.Handle(successfulParse, argument, nameof(TEnum));

            return result;
        }
    }
}