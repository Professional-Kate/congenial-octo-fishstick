using Console.Commands.Assertions;
using IdelPog.SimulationEngine.Structures;

namespace Console.Commands.Resolver
{
    public class ActionTypeResolver : IArgumentResolver<ActionType>
    {
        private readonly IAssertCanParseEnum _assertCanParse;

        public ActionTypeResolver(IAssertCanParseEnum assertCanParse)
        {
            _assertCanParse = assertCanParse;
        }

        public ActionType Resolve(string argument)
        {
            bool successfulParse = Enum.TryParse(argument, ignoreCase: true, out ActionType result);
            _assertCanParse.Handle(successfulParse, argument, nameof(ActionType));

            return result;
        }
    }
}