using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DeathSystem : IDeathSystem
    {
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatantAssertion _combatantAssertion;

        public DeathSystem(ICombatStateService combatStateService, ICombatantAssertion combatantAssertion)
        {
            _combatStateService = combatStateService;
            _combatantAssertion = combatantAssertion;
        }

        public void KillEntity(CombatantEntity combatantEntity)
        {
            _combatantAssertion.AssertCombatantAlive(combatantEntity);
            combatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });   
                
            _combatStateService.Evaluate();
        }
    }
}