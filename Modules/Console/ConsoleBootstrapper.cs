using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Command.Domain;
using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Console.Command.Mediator;
using IdelPog.Console.Command.Resolver;
using IdelPog.Console.Command.Resolver.Pipeline;
using IdelPog.Console.Factory;
using IdelPog.Console.Factory.Interface;
using IdelPog.Console.Runtime.ECS;
using IdelPog.Console.Runtime.Input;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.ECS.Assertion;
using IdelPog.ECS.Assertion.Interface;
using IdelPog.ECS.Entity;
using IdelPog.ECS.Factory;

namespace IdelPog.Console
{
    public static class ConsoleBootstrapper
    {
        public static IInputHandler Initialize(IBufferManager bufferManager)
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            ISpanAssertion spanAssertion = new SpanAssertion();
            IEnumParseAssertion enumParseAssertion = new EnumParseAssertion();
            ITypeParseAssertion typeParseAssertion = new TypeParseAssertion();
            IArgumentCountAssertion argumentCountAssertion = new ArgumentCountAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IComponentAssertion componentAssertion = new ComponentAssertion();
            IDomainPermissionAssertion domainPermissionAssertion = new DomainPermissionAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            INumberAssertion numberAssertion = new NumberAssertion();

            DomainComponent permissionDomain = new() { AllowedDomain = Domain.PERMISSION };
            IEntity allowedDomainEntity = new AllowedDomainsEntity(repositoryAsserter, [permissionDomain]);

            IAssetRepository<Domain, ICommandDomainResolver> commandRepository = new AssetRepository<Domain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(enumParseAssertion);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(enumParseAssertion);
            IArgumentResolver<Domain> commandDomainResolver = new EnumResolver<Domain>(enumParseAssertion);
            IArgumentResolver<uint> uIntResolver = new UIntResolver(typeParseAssertion, numberAssertion);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);

            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdate>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline = new CurrencyUpdateResolver(actionTypeResolver, uIntResolver, currencyTypeResolver);

            ICommandDomainResolver currencyDomainResolver = new CurrencyDomainResolver(currencyUpdatePipeline, currencyUpdateDispatcher, argumentCountAssertion);

            IComponentStoreFactory componentStoreFactory = new ComponentStoreFactory();
            IDomainComponentFactory domainComponentFactory = new DomainComponentFactory();

            IPermissionService permissionService = new PermissionService(allowedDomainEntity, domainComponentFactory, componentStoreFactory, componentAssertion);
            IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline = new PermissionUpdateResolver(actionTypeResolver, commandDomainResolver);
            ICommandDomainResolver permissionDomainResolver = new PermissionDomainResolver(permissionUpdatePipeline, permissionService, argumentCountAssertion);

            commandRepository.Add(Domain.CURRENCY, currencyDomainResolver);
            commandRepository.Add(Domain.PERMISSION, permissionDomainResolver);

            IDomainPermissionChecker domainPermissionChecker = new DomainPermissionChecker(allowedDomainEntity, componentAssertion);
            ICommandResolverMediator commandResolverMediator = new CommandResolverMediator(commandRepository, domainPermissionChecker, foundAssertion, spanAssertion, domainPermissionAssertion);

            IArgumentResolver<Domain> commandArgumentResolver = new EnumResolver<Domain>(enumParseAssertion);

            return new InputHandler(commandResolverMediator, commandArgumentResolver, spanAssertion);
        }
    }
}