using IdelPog.Console.Argument;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Schedule
{
    public class ScheduleDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SCHEDULE;

        private readonly EnumResolver<SubDomain> _subDomainResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public ScheduleDomainResolver(IArgumentCountAssertion argumentCountAssertion, IEnumParseAssertion enumParseAssertion)
        {
            _argumentCountAssertion = argumentCountAssertion;
            _subDomainResolver = new EnumResolver<SubDomain>(enumParseAssertion);
            
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 1);
            
            SubDomain subDomain = _subDomainResolver.Resolve(arguments[0]);
        }
    }
}