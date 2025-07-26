using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Commands.Domains.Arguments;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.ECS;
using Console.Runtime.Factory;
using Console.Runtime.Input;
using Console.Runtime.Systems;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
using IdelPog.ECS.Factory;
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
            ISpanAssertion spanAssertion = new SpanAssertion(throwHandler);
            IEnumParseAssertion enumParseAssertion = new EnumParseAssertion(throwHandler);
            ITypeParseAssertion typeParseAssertion = new TypeParseAssertion(throwHandler);
            IArgumentCountAssertion argumentCountAssertion = new ArgumentCountAssertion(throwHandler);
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(throwHandler);
            IAssertComponentFound assertComponentFound = new AssertComponentFound(throwHandler);
            IAssertComponentDoesNotExist assertComponentDoesNotExist = new AssertComponentDoesNotExist(throwHandler);
            IDomainPermissionAssertion domainPermissionAssertion = new DomainPermissionAssertion(throwHandler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);

            DomainComponent permissionDomain = new() { AllowedDomain = Domain.PERMISSION };
            IEntity allowedDomainEntity = new AllowedDomainsEntity([permissionDomain]);

            IAssetRepository<Domain, ICommandDomainResolver> commandRepository = new AssetRepository<Domain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(enumParseAssertion);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(enumParseAssertion);
            IArgumentResolver<SkillID> skillIDResolver = new EnumResolver<SkillID>(enumParseAssertion);
            IArgumentResolver<Domain> commandDomainResolver = new EnumResolver<Domain>(enumParseAssertion);
            IArgumentResolver<ControlAction> controlActionResolver = new EnumResolver<ControlAction>(enumParseAssertion);
            IArgumentResolver<int> intResolver = new IntResolver(typeParseAssertion);

            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdate>(bufferManager, assertNotNull, assertCollectionNotEmpty);

            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline =
                new CurrencyUpdateResolver(actionTypeResolver, intResolver, currencyTypeResolver);

            ICommandDomainResolver currencyDomainResolver =
                new CurrencyDomainResolver(currencyUpdatePipeline, currencyUpdateFactory, currencyUpdateDispatcher, argumentCountAssertion);

            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory();
            IDispatchOne<SkillChange> skillChangeDispatcher = new ManagedDispatcher<SkillChange>(bufferManager, assertNotNull, assertCollectionNotEmpty);

            IArgumentResolverPipeline<SkillChangeArguments> skillChangePipeline = new SkillChangeResolver(skillIDResolver);
            ICommandDomainResolver skillDomainResolver =
                new SkillDomainResolver(skillChangePipeline, skillChangeDispatcher, skillChangeFactory, argumentCountAssertion);

            IComponentStoreFactory componentStoreFactory = new ComponentStoreFactory();
            IDomainComponentFactory domainComponentFactory = new DomainComponentFactory();

            IPermissionService permissionService = new PermissionService(allowedDomainEntity, domainComponentFactory, componentStoreFactory, assertComponentFound,
                assertComponentDoesNotExist);

            IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline =
                new PermissionUpdateResolver(actionTypeResolver, commandDomainResolver);

            ICommandDomainResolver permissionDomainResolver = new PermissionDomainResolver(permissionUpdatePipeline, permissionService, argumentCountAssertion);

            IScheduleControlFactory scheduleControlFactory = new ScheduleControlFactory();
            IDispatchOne<ScheduleControl> scheduleControlDispatcher =
                new ManagedDispatcher<ScheduleControl>(bufferManager, assertNotNull, assertCollectionNotEmpty);

            IArgumentResolverPipeline<ScheduleControlArguments> scheduleControlPipeline = new ScheduleControlResolver(controlActionResolver);
            ICommandDomainResolver scheduleDomainResolver =
                new ScheduleDomainResolver(scheduleControlPipeline, scheduleControlDispatcher, scheduleControlFactory, argumentCountAssertion);

            commandRepository.Add(Domain.CURRENCY, currencyDomainResolver);
            commandRepository.Add(Domain.SKILL, skillDomainResolver);
            commandRepository.Add(Domain.PERMISSION, permissionDomainResolver);
            commandRepository.Add(Domain.SCHEDULE, scheduleDomainResolver);

            IDomainPermissionChecker domainPermissionChecker = new DomainPermissionChecker(allowedDomainEntity, assertComponentFound);
            ICommandResolverMediator commandResolverMediator =
                new CommandResolverMediator(commandRepository, domainPermissionChecker, assertFound, spanAssertion, domainPermissionAssertion);

            IArgumentResolver<Domain> commandArgumentResolver = new EnumResolver<Domain>(enumParseAssertion);

            return new InputHandler(commandResolverMediator, commandArgumentResolver, spanAssertion);
        }
    }
}