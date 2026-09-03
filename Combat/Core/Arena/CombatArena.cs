using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Core.Arena
{
    public sealed class CombatArena : ICombatArena
    {
        // TODO: these three need to be created per run
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAbilityEntityRepository _abilityEntityRepository;
        private readonly ICombatantEntityFactory _combatantEntityFactory;
        // ICombatQueue (this is used in the runner but imagine it's here)
        
        private readonly IDictionary<byte, EquippedAbilityDefinition> _equippedAbilityDictionary;
        private readonly IAbilityEntityFactory _abilityEntityFactory;
        private readonly IInitialAbilityScheduler _initialAbilityScheduler;
        private readonly ICombatQueueRunner _combatQueueRunner;

        public CombatArena(ICombatantEntityFactory combatantEntityFactory, ICombatantRepository combatantRepository,
            IDictionary<byte, EquippedAbilityDefinition> equippedAbilityDictionary, IAbilityEntityFactory abilityEntityFactory,
            IAbilityEntityRepository abilityEntityRepository, IInitialAbilityScheduler initialAbilityScheduler, ICombatQueueRunner combatQueueRunner)
        {
            _combatantEntityFactory = combatantEntityFactory;
            _combatantRepository = combatantRepository;
            _equippedAbilityDictionary = equippedAbilityDictionary;
            _abilityEntityFactory = abilityEntityFactory;
            _abilityEntityRepository = abilityEntityRepository;
            _initialAbilityScheduler = initialAbilityScheduler;
            _combatQueueRunner = combatQueueRunner;
        }

        public void RunCombatSimulation(IReadOnlyList<CombatantDefinition> friendlyCombatantDefinitions, IReadOnlyList<CombatantDefinition> enemyCombatantDefinitions)
        {
            CombatantEntity[] friendlyCombatants = _combatantEntityFactory.Create(friendlyCombatantDefinitions, TargetingType.FRIENDLY);
            CombatantEntity[] enemyCombatants = _combatantEntityFactory.Create(enemyCombatantDefinitions, TargetingType.ENEMY);
            
            _combatantRepository.SeedFriendlyCombatants(friendlyCombatants);
            _combatantRepository.SeedEnemyCombatants(enemyCombatants);
            
            List<AbilityEntity> abilityEntities = [];
            foreach (CombatantEntity combatantEntity in _combatantRepository.Enumerate())
            {
                if (_equippedAbilityDictionary.TryGetValue(combatantEntity.CombatantID, out EquippedAbilityDefinition equippedAbilityDefinition) == false)
                {
                    continue;
                }

                abilityEntities.AddRange(_abilityEntityFactory.Create(equippedAbilityDefinition, combatantEntity.InstanceID));
            }
            
            _abilityEntityRepository.SeedAbilities(abilityEntities.ToArray());
            
            _initialAbilityScheduler.ScheduleRegisteredAbilities(initialTick: 0);
            _combatQueueRunner.RunCombat();
            
            _abilityEntityRepository.Clear();
            _combatantRepository.Clear();
        }
    }
}