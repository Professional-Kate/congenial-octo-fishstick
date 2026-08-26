using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TearDownService : ITearDownService
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityRepository;
        private readonly ICombatQueueClear _combatQueueClear;

        public TearDownService(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityRepository, ICombatQueueClear combatQueueClear)
        {
            _combatantRepository = combatantRepository;
            _combatantAbilityRepository = combatantAbilityRepository;
            _combatQueueClear = combatQueueClear;
        }

        public void TearDownState()
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAllParticipating())
            {
                TearDownCombatant(combatantEntity);

                if (_combatantAbilityRepository.Contains(combatantEntity.CombatantID))
                {
                    TearDownCombatantAbilities(_combatantAbilityRepository.GetAll(combatantEntity.CombatantID));
                }
            }
            
            _combatQueueClear.Clear();
        }

        private static void TearDownCombatant(CombatantEntity combatantEntity)
        {
            combatantEntity.RemoveComponent<TargetingTypeComponent>();
            combatantEntity.RemoveComponent<CombatParticipantComponent>();

            if (combatantEntity.ContainsComponent<RetaliationComponent>())
            { 
                combatantEntity.RemoveComponent<RetaliationComponent>();
            }
            
            BaseHealthComponent baseHealthComponent = combatantEntity.GetComponent<BaseHealthComponent>();
            combatantEntity.ReplaceComponent(new HealthComponent { Health = baseHealthComponent.Health });
            
            combatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = true });
        }
        
        private static void TearDownCombatantAbilities(IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        { 
            foreach (CombatantAbilityEntity abilityEntity in combatantAbilityEntities)
            { 
                abilityEntity.RemoveComponent<ReadyTickComponent>();
            }
        }
    }
}