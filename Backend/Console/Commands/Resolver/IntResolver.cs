using Console.Commands.Resolver.Assertions;

namespace Console.Commands.Resolver
{
    public class IntResolver : IArgumentResolver<int>
    {
        private readonly IAssertCanParseType  _assertCanParseType;

        public IntResolver(IAssertCanParseType assertCanParseType)
        {
            _assertCanParseType = assertCanParseType;
        }

        public int Resolve(string argument)
        {
            bool successfulParse = int.TryParse(argument, out int result);
            _assertCanParseType.Handle(successfulParse, argument, typeof(int));

            return result;
        }
    }
}