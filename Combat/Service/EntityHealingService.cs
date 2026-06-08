using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class EntityHealingService : IEntityHealingService
    {
        public void ApplyHealing(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity healingCombatant, CombatantAbilityEntity healingAbility, double tick)
        {
            throw new NotImplementedException();
        }
    }
}