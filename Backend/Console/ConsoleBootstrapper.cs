using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.Input;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console
{
    public static class ConsoleBootstrapper
    {
        public static IInputHandler Initialize(IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            IAssertFound assertFound = new AssertFound(throwHandler);
            IAssertNotNull assertNotNull = new AssertNotNull(throwHandler);
            IAssertNonDuplicate assertNonDuplicate = new AssertNonDuplicate(throwHandler);
            IAssertSpanNotEmpty assertSpanNotEmpty = new AssertSpanNotEmpty(throwHandler);
            IAssertCanParseEnum assertCanParseEnum = new AssertCanParseEnum(throwHandler);
            IAssertCanParseType assertCanParseType = new AssertCanParseType(throwHandler);
            IAssertArgumentLength assertArgumentLength = new AssertArgumentLength(throwHandler);
            IAssertCollectionNotEmpty  assertCollectionNotEmpty = new AssertCollectionNotEmpty(throwHandler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);
            
            IAssetRepository<CommandDomain, ICommandDomainResolver> commandRepository = new AssetRepository<CommandDomain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(assertCanParseEnum);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(assertCanParseEnum);
            IArgumentResolver<int> intResolver = new IntResolver(assertCanParseType);
            
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdate>(bufferManager, assertNotNull, assertCollectionNotEmpty);
                
            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline = new CurrencyUpdateResolver(actionTypeResolver, intResolver, currencyTypeResolver);
            ICommandDomainResolver currencyDomainResolver = new CurrencyDomainResolver(currencyUpdatePipeline, assertArgumentLength, currencyUpdateFactory, currencyUpdateDispatcher);
            
            commandRepository.Add(CommandDomain.CURRENCY, currencyDomainResolver);
            
            ICommandResolverMediator commandResolverMediator = new CommandResolverMediator(commandRepository, assertFound, assertSpanNotEmpty);

            IArgumentResolver<CommandDomain> commandArgumentResolver = new EnumResolver<CommandDomain>(assertCanParseEnum);
            
            return new InputHandler(commandResolverMediator, commandArgumentResolver, assertSpanNotEmpty);
        }
    }
}