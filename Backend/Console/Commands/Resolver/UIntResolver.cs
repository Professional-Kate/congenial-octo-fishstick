using Console.Assertions;

namespace Console.Commands.Resolver
{
    public class UIntResolver : IArgumentResolver<uint>
    {
        private readonly ITypeParseAssertion _typeParseAssertion;
        private readonly INumberAssertion _numberAssertion;

        public UIntResolver(ITypeParseAssertion typeParseAssertion, INumberAssertion numberAssertion)
        {
            _typeParseAssertion = typeParseAssertion;
            _numberAssertion = numberAssertion;
        }

        public uint Resolve(string argument)
        {
            bool successfulParse = int.TryParse(argument, out int parsedInt);
            _typeParseAssertion.AssertCanParse(successfulParse, argument, typeof(int));
            _numberAssertion.AssertNonNegative(parsedInt);
            
            uint result = (uint) parsedInt;
            return result;
        }
    }
}