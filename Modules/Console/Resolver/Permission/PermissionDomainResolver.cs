using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Permission
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.PERMISSION;

        private readonly IArgumentResolverPipeline<PermissionUpdateArguments> _permissionUpdateResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public PermissionDomainResolver(IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdateResolver, IArgumentCountAssertion argumentCountAssertion)
        {
            _permissionUpdateResolver = permissionUpdateResolver;
            _argumentCountAssertion = argumentCountAssertion;
            
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);
            
            _permissionUpdateResolver.Resolve(arguments);
        }
    }
}