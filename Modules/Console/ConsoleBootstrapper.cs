using IdelPog.Console.Argument;
using IdelPog.Console.Argument.Interface;
using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Mediator;
using IdelPog.Console.Resolver;
using IdelPog.Console.Resolver.Currency;
using IdelPog.Console.Resolver.Permission;
using IdelPog.Console.Resolver.Schedule;
using IdelPog.Console.Resolver.Skill;
using IdelPog.Console.Runtime;
using IdelPog.Console.Runtime.Input;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
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
            IHandler throwHandler = new ThrowHandler();
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            ISpanAssertion spanAssertion = new SpanAssertion(throwHandler);
            IEnumParseAssertion enumParseAssertion = new EnumParseAssertion(throwHandler);
            ITypeParseAssertion typeParseAssertion = new TypeParseAssertion(throwHandler);
            IArgumentCountAssertion argumentCountAssertion = new ArgumentCountAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IComponentAssertion componentAssertion = new ComponentAssertion(throwHandler);
            IDomainPermissionAssertion domainPermissionAssertion = new DomainPermissionAssertion(throwHandler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            INumberAssertion numberAssertion = new NumberAssertion(throwHandler);

            DomainComponent permissionDomain = new() { AllowedDomain = Domain.PERMISSION };
            IEntity allowedDomainEntity = new AllowedDomainsEntity([permissionDomain]);

            IAssetRepository<Domain, ICommandDomainResolver> commandRepository = new AssetRepository<Domain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(enumParseAssertion);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(enumParseAssertion);
            IArgumentResolver<SkillID> skillIDResolver = new EnumResolver<SkillID>(enumParseAssertion);
            IArgumentResolver<Domain> commandDomainResolver = new EnumResolver<Domain>(enumParseAssertion);
            IArgumentResolver<ControlAction> controlActionResolver = new EnumResolver<ControlAction>(enumParseAssertion);
            IArgumentResolver<uint> uIntResolver = new UIntResolver(typeParseAssertion, numberAssertion);

            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher =
                new ManagedDispatcher<CurrencyUpdate>(bufferManager, objectNullAssertion, collectionAssertion);

            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline =
                new CurrencyUpdateResolver(actionTypeResolver, uIntResolver, currencyTypeResolver);

            ICommandDomainResolver currencyDomainResolver =
                new CurrencyDomainResolver(argumentCountAssertion, enumParseAssertion);

            IDispatchOne<SetSkill> skillChangeDispatcher = new ManagedDispatcher<SetSkill>(bufferManager, objectNullAssertion, collectionAssertion);

            IArgumentResolverPipeline<SetSkillArguments> skillChangePipeline = new SetSkillResolver(skillIDResolver);
            ICommandDomainResolver skillDomainResolver =
                new SkillDomainResolver(argumentCountAssertion, enumParseAssertion);

            IComponentStoreFactory componentStoreFactory = new ComponentStoreFactory();

            IPermissionService permissionService = new PermissionService(allowedDomainEntity, componentStoreFactory, componentAssertion);

            IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline =
                new PermissionUpdateResolver(actionTypeResolver, commandDomainResolver);

            ICommandDomainResolver permissionDomainResolver = new PermissionDomainResolver(argumentCountAssertion, enumParseAssertion);

            IDispatchOne<ScheduleControl> scheduleControlDispatcher =
                new ManagedDispatcher<ScheduleControl>(bufferManager, objectNullAssertion, collectionAssertion);

            IArgumentResolverPipeline<ScheduleControlArguments> scheduleControlPipeline = new ScheduleControlResolver(controlActionResolver);
            ICommandDomainResolver scheduleDomainResolver =
                new ScheduleDomainResolver(argumentCountAssertion, enumParseAssertion);

            commandRepository.Add(Domain.CURRENCY, currencyDomainResolver);
            commandRepository.Add(Domain.SKILL, skillDomainResolver);
            commandRepository.Add(Domain.PERMISSION, permissionDomainResolver);
            commandRepository.Add(Domain.SCHEDULE, scheduleDomainResolver);

            IDomainPermissionChecker domainPermissionChecker = new DomainPermissionChecker(allowedDomainEntity, componentAssertion);
            ICommandResolverMediator commandResolverMediator =
                new CommandResolverMediator(commandRepository, domainPermissionChecker, foundAssertion, spanAssertion, domainPermissionAssertion);

            IArgumentResolver<Domain> commandArgumentResolver = new EnumResolver<Domain>(enumParseAssertion);

            return new InputHandler(commandResolverMediator, commandArgumentResolver, spanAssertion);
        }
    }
}