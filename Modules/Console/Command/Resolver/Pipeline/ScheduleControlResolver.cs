using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Resolver.Pipeline
{
    public class ScheduleControlResolver : IArgumentResolverPipeline<ScheduleControlArguments>
    {
        private readonly IArgumentResolver<ControlAction> _controlActionResolver;

        public ScheduleControlResolver(IArgumentResolver<ControlAction> controlActionResolver)
        {
            _controlActionResolver = controlActionResolver;
        }

        public ScheduleControlArguments Resolve(ReadOnlySpan<string> arguments)
        {
            ControlAction controlAction = _controlActionResolver.Resolve(arguments[0]);

            return new ScheduleControlArguments
            {
                ControlAction = controlAction
            };
        }
    }
}