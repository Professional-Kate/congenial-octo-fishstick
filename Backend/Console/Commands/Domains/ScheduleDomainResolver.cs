using Console.Commands.Domains.Arguments;
using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;

namespace Console.Commands.Domains
{
    public class ScheduleDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SCHEDULE;
        public CommandDocumentation CommandDocumentation { get; } =
            new() { Syntax = "schedule <ControlAction>", Description = "Stop or start the automatic scheduler" };

        private readonly IArgumentResolverPipeline<ScheduleControlArguments> _controlActionResolver;
        private readonly IDispatchOne<ScheduleControl> _scheduleControlDispatcher;
        private readonly IScheduleControlFactory _scheduleControlFactory;
        private readonly IAssertArgumentLength _assertArgumentLength;

        public ScheduleDomainResolver(IArgumentResolverPipeline<ScheduleControlArguments> controlActionResolver,
            IDispatchOne<ScheduleControl> scheduleControlDispatcher, IScheduleControlFactory scheduleControlFactory, IAssertArgumentLength assertArgumentLength)
        {
            _controlActionResolver = controlActionResolver;
            _scheduleControlDispatcher = scheduleControlDispatcher;
            _scheduleControlFactory = scheduleControlFactory;
            _assertArgumentLength = assertArgumentLength;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _assertArgumentLength.Handle(arguments.Length, 1);
            ScheduleControlArguments scheduleControlArguments = _controlActionResolver.Resolve(arguments);

            _scheduleControlDispatcher.Dispatch(_scheduleControlFactory.Create(scheduleControlArguments.ControlAction));
        }
    }
}