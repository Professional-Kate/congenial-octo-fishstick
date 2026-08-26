using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEntityDamageService
    {
        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, byte initiatingCombatantID, CombatantAbilityStage combatantAbilityStage, double tick);
    }
}