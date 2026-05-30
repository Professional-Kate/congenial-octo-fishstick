using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TearDownService : ITearDownService
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatQueueClear _combatQueueClear;

        public TearDownService(ICombatantRepository combatantRepository, ICombatQueueClear combatQueueClear)
        {
            _combatantRepository = combatantRepository;
            _combatQueueClear = combatQueueClear;
        }

        public void ResetCombatants()
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            {
                combatantEntity.RemoveComponent<FriendlyStatusComponent>();
                
                BaseStatsComponent baseStatsComponent = combatantEntity.GetComponent<BaseStatsComponent>();
                combatantEntity.ReplaceComponent(baseStatsComponent.GetStats);
                
                combatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = true });
            }
            
            _combatQueueClear.Clear();
        }
    }
}