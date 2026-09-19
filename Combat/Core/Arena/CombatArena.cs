using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Core.Arena
{
    public sealed class CombatArena : ICombatArena
    {
        // TODO: these need to be created per run
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAbilityEntityRepository _abilityEntityRepository;
        private readonly ICombatantEntityFactory _combatantEntityFactory;
        private readonly IInitialStateLogger _initialStateLogger;
        private readonly ICombatantLogger _combatantLogger;
        private readonly ICombatStateService _combatStateService;
        // ICombatQueue (this is used in the runner but imagine it's here)
        
        private readonly IDictionary<byte, EquippedAbilityDefinition> _equippedAbilityDictionary;
        private readonly IAbilityEntityFactory _abilityEntityFactory;
        private readonly IInitialAbilityScheduler _initialAbilityScheduler;
        private readonly ICombatQueueRunner _combatQueueRunner;

        public CombatArena(ICombatantEntityFactory combatantEntityFactory, ICombatantRepository combatantRepository,
            IDictionary<byte, EquippedAbilityDefinition> equippedAbilityDictionary, IAbilityEntityFactory abilityEntityFactory,
            IAbilityEntityRepository abilityEntityRepository, IInitialAbilityScheduler initialAbilityScheduler, ICombatQueueRunner combatQueueRunner, IInitialStateLogger initialStateLogger, ICombatantLogger combatantLogger, ICombatStateService combatStateService)
        {
            _combatantEntityFactory = combatantEntityFactory;
            _combatantRepository = combatantRepository;
            _equippedAbilityDictionary = equippedAbilityDictionary;
            _abilityEntityFactory = abilityEntityFactory;
            _abilityEntityRepository = abilityEntityRepository;
            _initialAbilityScheduler = initialAbilityScheduler;
            _combatQueueRunner = combatQueueRunner;
            _initialStateLogger = initialStateLogger;
            _combatantLogger = combatantLogger;
            _combatStateService = combatStateService;
        }

        public CombatArenaLog RunCombatSimulation(IReadOnlyList<CombatantDefinition> friendlyCombatantDefinitions, IReadOnlyList<CombatantDefinition> enemyCombatantDefinitions)
        {
            CombatantEntity[] friendlyCombatants = _combatantEntityFactory.Create(friendlyCombatantDefinitions, TargetingType.FRIENDLY);
            CombatantEntity[] enemyCombatants = _combatantEntityFactory.Create(enemyCombatantDefinitions, TargetingType.ENEMY);
            AbilityEntity[] abilityEntities = CreateAbilityEntities(friendlyCombatants, enemyCombatants);
            
            InitialEntities initialEntities = _initialStateLogger.LogInitialState(friendlyCombatants, enemyCombatants, abilityEntities);

            try
            {
                _combatantRepository.SeedFriendlyCombatants(friendlyCombatants);
                _combatantRepository.SeedEnemyCombatants(enemyCombatants);
                _abilityEntityRepository.SeedAbilities(abilityEntities);
                
                _initialAbilityScheduler.ScheduleRegisteredAbilities(initialTick: 0);
                _combatQueueRunner.RunCombat();

                return new CombatArenaLog
                {
                    InitialEntities = initialEntities,
                    CombatStages = _combatantLogger.GetStateChanges(),
                    FriendlyVictory = _combatStateService.FriendlyVictory
                };
            }
            finally
            {
                _abilityEntityRepository.Clear();
                _combatantRepository.Clear();
                // TODO : we need to create the state service per CombatArena, then we can remove this
                _combatStateService.Reset();
            }
        }

        private AbilityEntity[] CreateAbilityEntities(CombatantEntity[] friendlyCombatants, CombatantEntity[] enemyCombatants)
        {
            // if profiling ever complains about this line then change the code to run per input array instead
            CombatantEntity[] combinedCombatants = [..friendlyCombatants, ..enemyCombatants];
            
            List<AbilityEntity> abilityEntities = [];
            foreach (CombatantEntity combatantEntity in combinedCombatants)
            {
                if (_equippedAbilityDictionary.TryGetValue(combatantEntity.CombatantID, out EquippedAbilityDefinition equippedAbilityDefinition) == false)
                {
                    continue;
                }

                abilityEntities.AddRange(_abilityEntityFactory.Create(equippedAbilityDefinition, combatantEntity.InstanceID));
            }
            
            return  abilityEntities.ToArray();
        }
    }
}