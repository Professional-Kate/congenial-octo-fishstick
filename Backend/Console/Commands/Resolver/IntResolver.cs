using Console.Assertions;

namespace Console.Commands.Resolver
{
    public class IntResolver : IArgumentResolver<int>
    {
        private readonly ITypeParseAssertion _typeParseAssertion;

        public IntResolver(ITypeParseAssertion typeParseAssertion)
        {
            _typeParseAssertion = typeParseAssertion;
        }

        public int Resolve(string argument)
        {
            bool successfulParse = int.TryParse(argument, out int result);
            _typeParseAssertion.AssertCanParse(successfulParse, argument, typeof(int));

            return result;
        }
    }
}