using IdelPog.Console.Argument.Interface;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Schedule
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