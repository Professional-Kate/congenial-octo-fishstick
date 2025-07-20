using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.ECS;
using Console.Runtime.Input;
using Console.Runtime.Systems;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
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
            IAssertHasPermission assertHasPermission = new AssertHasPermission(throwHandler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);
            
            IAssetRepository<CommandDomain, ICommandDomainResolver> commandRepository = new AssetRepository<CommandDomain, ICommandDomainResolver>(repositoryAsserter);

            IArgumentResolver<ActionType> actionTypeResolver = new EnumResolver<ActionType>(assertCanParseEnum);
            IArgumentResolver<CurrencyType> currencyTypeResolver = new EnumResolver<CurrencyType>(assertCanParseEnum);
            IArgumentResolver<int> intResolver = new IntResolver(assertCanParseType);
            IArgumentResolver<SkillID> skillIDResolver = new EnumResolver<SkillID>(assertCanParseEnum);
            
            ICurrencyUpdateFactory currencyUpdateFactory = new CurrencyUpdateFactory();
            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher = new ManagedDispatcher<CurrencyUpdate>(bufferManager, assertNotNull, assertCollectionNotEmpty);
                
            IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline = new CurrencyUpdateResolver(actionTypeResolver, intResolver, currencyTypeResolver);
            ICommandDomainResolver currencyDomainResolver = new CurrencyDomainResolver(currencyUpdatePipeline, currencyUpdateFactory, currencyUpdateDispatcher, assertArgumentLength);

            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory(); 
            IDispatchOne<SkillChange> skillChangeDispatcher = new ManagedDispatcher<SkillChange>(bufferManager, assertNotNull, assertCollectionNotEmpty);
            
            IArgumentResolverPipeline<SkillChangeArguments> skillChangePipeline = new SkillChangeResolver(skillIDResolver);
            ICommandDomainResolver skillDomainResolver = new SkillDomainResolver(skillChangePipeline, skillChangeDispatcher, skillChangeFactory, assertArgumentLength);
            
            commandRepository.Add(CommandDomain.CURRENCY, currencyDomainResolver);
            commandRepository.Add(CommandDomain.SKILL, skillDomainResolver);

            CommandDomainComponent currencyDomainComponent = new() { AllowedCommandDomain = CommandDomain.CURRENCY};
            CommandDomainComponent skillDomainComponent = new() { AllowedCommandDomain = CommandDomain.SKILL};
            IEntity allowedDomainEntity = new AllowedDomainsEntity([skillDomainComponent]);
            IDomainPermissionChecker domainPermissionChecker = new DomainPermissionChecker(allowedDomainEntity, assertComponentFound);            
            ICommandResolverMediator commandResolverMediator = new CommandResolverMediator(commandRepository, domainPermissionChecker, assertFound, assertSpanNotEmpty, assertHasPermission);

            IArgumentResolver<CommandDomain> commandArgumentResolver = new EnumResolver<CommandDomain>(assertCanParseEnum);
            
            return new InputHandler(commandResolverMediator, commandArgumentResolver, assertSpanNotEmpty);
        }
    }
}