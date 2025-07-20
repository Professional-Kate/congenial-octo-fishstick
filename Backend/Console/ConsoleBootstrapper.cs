using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.ECS;
using Console.Runtime.Factory;
using Console.Runtime.Input;
using Console.Runtime.Systems;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
using IdelPog.ECS.Factory;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Skill;
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
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(throwHandler);
            IAssertComponentFound assertComponentFound = new AssertComponentFound(throwHandler);
            IAssertComponentDoesNotExist assertComponentDoesNotExist = new AssertComponentDoesNotExist(throwHandler);
            IAssertHasPermission assertHasPermission = new AssertHasPermission(throwHandler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);
            
            DomainComponent currencyDomainComponent = new() { AllowedDomain = Domain.CURRENCY};
            DomainComponent skillDomainComponent = new() { AllowedDomain = Domain.SKILL};
            DomainComponent permissionDomain = new() { AllowedDomain = Domain.PERMISSION};
            IEntity allowedDomainEntity = new AllowedDomainsEntity([permissionDomain, currencyDomainComponent, skillDomainComponent]);
            
            IAssetRepository<Domain, ICommandDomainResolver> commandRepository = new AssetRepository<Domain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(assertCanParseEnum);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(assertCanParseEnum);
            IArgumentResolver<SkillID> skillIDResolver = new EnumResolver<SkillID>(assertCanParseEnum);
            IArgumentResolver<Domain> commandDomainResolver = new EnumResolver<Domain>(assertCanParseEnum);
            IArgumentResolver<int> intResolver = new IntResolver(assertCanParseType);
            
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdate>(bufferManager, assertNotNull, assertCollectionNotEmpty);
                
            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline = new CurrencyUpdateResolver(actionTypeResolver, intResolver, currencyTypeResolver);
            ICommandDomainResolver currencyDomainResolver = new CurrencyDomainResolver(currencyUpdatePipeline, currencyUpdateFactory, currencyUpdateDispatcher, assertArgumentLength);

            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory(); 
            IDispatchOne<SkillChange> skillChangeDispatcher = new ManagedDispatcher<SkillChange>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            
            IArgumentResolverPipeline<SkillChangeArguments> skillChangePipeline = new SkillChangeResolver(skillIDResolver);
            ICommandDomainResolver skillDomainResolver = new SkillDomainResolver(skillChangePipeline, skillChangeDispatcher, skillChangeFactory, assertArgumentLength);

            IComponentStoreFactory componentStoreFactory = new ComponentStoreFactory();
            IDomainComponentFactory domainComponentFactory = new DomainComponentFactory();
            
            IPermissionService permissionService = new PermissionService(allowedDomainEntity, domainComponentFactory, componentStoreFactory, assertComponentFound, assertComponentDoesNotExist);

            IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline = new PermissionUpdateResolver(actionTypeResolver,commandDomainResolver);
            ICommandDomainResolver permissionDomainResolver = new PermissionDomainResolver(permissionUpdatePipeline, permissionService, assertArgumentLength);
            
            commandRepository.Add(Domain.CURRENCY, currencyDomainResolver);
            commandRepository.Add(Domain.SKILL, skillDomainResolver);
            commandRepository.Add(Domain.PERMISSION, permissionDomainResolver);
           
            IDomainPermissionChecker domainPermissionChecker = new DomainPermissionChecker(allowedDomainEntity, assertComponentFound);            
            ICommandResolverMediator commandResolverMediator = new CommandResolverMediator(commandRepository, domainPermissionChecker, assertFound, assertSpanNotEmpty, assertHasPermission);

            IArgumentResolver<Domain> commandArgumentResolver = new EnumResolver<Domain>(assertCanParseEnum);
            
            return new InputHandler(commandResolverMediator, commandArgumentResolver, assertSpanNotEmpty);
        }
    }
}