using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Service.Interface
{
    public interface IEntityDamageService
    {
        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, byte initiatingCombatantID, AbilityStage abilityStage, double tick);
    }
}