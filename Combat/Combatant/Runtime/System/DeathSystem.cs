using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Combatant.Runtime.System
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