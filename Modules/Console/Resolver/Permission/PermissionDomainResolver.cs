using IdelPog.Console.Argument;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Permission
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.PERMISSION;

        private readonly EnumResolver<SubDomain> _subDomainResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public PermissionDomainResolver(IArgumentCountAssertion argumentCountAssertion, IEnumParseAssertion enumParseAssertion)
        {
            _argumentCountAssertion = argumentCountAssertion;
            _subDomainResolver = new EnumResolver<SubDomain>(enumParseAssertion);
            
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);

            SubDomain subDomain = _subDomainResolver.Resolve(arguments[0]);
        }
    }
}