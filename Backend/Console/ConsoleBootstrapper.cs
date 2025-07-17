using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Assertions;
using Console.Runtime.Input;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console
{
    public static class ConsoleBootstrapper
    {
        public static IInputHandler Initialize()
        {
            IHandler throwHandler = new ThrowHandler();
            IAssertFound assertFound = new AssertFound(throwHandler);
            IAssertNotNull assertNotNull = new AssertNotNull(throwHandler);
            IAssertNonDuplicate assertNonDuplicate = new AssertNonDuplicate(throwHandler);
            IAssertSpanNotEmpty assertSpanNotEmpty = new AssertSpanNotEmpty(throwHandler);
            IAssertCanParseEnum assertCanParseEnum = new AssertCanParseEnum(throwHandler);
            
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);
            IAssetRepository<CommandDomain, ICommandDomainResolver> commandRepository = new AssetRepository<CommandDomain, ICommandDomainResolver>(repositoryAsserter);
            ICommandResolverMediator commandResolverMediator = new CommandResolverMediator(commandRepository, assertFound, assertSpanNotEmpty);

            IArgumentResolver<CommandDomain> commandArgumentResolver = new EnumResolver<CommandDomain>(assertCanParseEnum);
            
            return new InputHandler(commandResolverMediator, commandArgumentResolver, assertSpanNotEmpty);
        }
    }
}