using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DeathSystem : IDeathSystem
    {
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly ICombatantAssertion _combatantAssertion;

        public DeathSystem(ICombatStateService combatStateService, ICombatantStoreService combatantStoreService, ICombatantAssertion combatantAssertion)
        {
            _combatStateService = combatStateService;
            _combatantStoreService = combatantStoreService;
            _combatantAssertion = combatantAssertion;
        }

        public void KillEntity(CombatantEntity combatantEntity)
        {
            _combatantAssertion.AssertCombatantAlive(combatantEntity);
            combatantEntity.UpdateLifeStatus(false);    
                
            _combatStateService.Evaluate(combatantEntity);
            if (_combatStateService.IsCombatOver)
            {
                return;
            }
                
            _combatantStoreService.RegisterCombatantDeath(combatantEntity);
        }
    }
}