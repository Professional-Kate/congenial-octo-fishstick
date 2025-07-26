using Console.Commands.Domains.Arguments;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver.Pipelines
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