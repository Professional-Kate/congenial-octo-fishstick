using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Console.Command.Resolver.Pipeline;
using IdelPog.Console.Factory.Interface;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Single;

namespace IdelPog.Console.Command.Domain
{
    public class ScheduleDomainResolver : ICommandDomainResolver
    {
        public Types.Domain HandledDomain => Types.Domain.SCHEDULE;
        public CommandDocumentation CommandDocumentation { get; } =
            new() { Syntax = "schedule <ControlAction>", Description = "Stop or start the automatic scheduler" };

        private readonly IArgumentResolverPipeline<ScheduleControlArguments> _controlActionResolver;
        private readonly IDispatchOne<ScheduleControl> _scheduleControlDispatcher;
        private readonly IScheduleControlFactory _scheduleControlFactory;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public ScheduleDomainResolver(IArgumentResolverPipeline<ScheduleControlArguments> controlActionResolver,
            IDispatchOne<ScheduleControl> scheduleControlDispatcher, IScheduleControlFactory scheduleControlFactory,
            IArgumentCountAssertion argumentCountAssertion)
        {
            _controlActionResolver = controlActionResolver;
            _scheduleControlDispatcher = scheduleControlDispatcher;
            _scheduleControlFactory = scheduleControlFactory;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 1);
            ScheduleControlArguments scheduleControlArguments = _controlActionResolver.Resolve(arguments);

            _scheduleControlDispatcher.Dispatch(_scheduleControlFactory.Create(scheduleControlArguments.ControlAction));
        }
    }
}