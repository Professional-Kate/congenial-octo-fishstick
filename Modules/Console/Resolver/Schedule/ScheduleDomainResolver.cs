using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Schedule
{
    public class ScheduleDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SCHEDULE;

        private readonly IArgumentResolverPipeline<ScheduleControlArguments> _scheduleControlResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public ScheduleDomainResolver(IArgumentResolverPipeline<ScheduleControlArguments> scheduleControlResolver, IArgumentCountAssertion argumentCountAssertion)
        {
            _argumentCountAssertion = argumentCountAssertion;
            _scheduleControlResolver = scheduleControlResolver;
            
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 1);
            
            _scheduleControlResolver.Resolve(arguments);
        }
    }
}