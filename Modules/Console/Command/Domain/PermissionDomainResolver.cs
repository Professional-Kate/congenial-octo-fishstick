using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Console.Command.Resolver.Pipeline;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;

namespace IdelPog.Console.Command.Domain
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public Types.Domain HandledDomain => Types.Domain.PERMISSION;
        public CommandDocumentation CommandDocumentation => new()
            { Syntax = "permission <ActionType> <CommandDomain>", Description = "Add or Remove permission for a domain" };

        private readonly IArgumentResolverPipeline<PermissionUpdateArguments> _permissionUpdatePipeline;
        private readonly IArgumentCountAssertion _argumentCountAssertion;
        private readonly IPermissionService _permissionService;

        public PermissionDomainResolver(IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline, IPermissionService permissionService,
            IArgumentCountAssertion argumentCountAssertion)
        {
            _permissionUpdatePipeline = permissionUpdatePipeline;
            _permissionService = permissionService;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);

            PermissionUpdateArguments permissionUpdateArguments = _permissionUpdatePipeline.Resolve(arguments);
            _permissionService.PermissionUpdate(permissionUpdateArguments);
        }
    }
}