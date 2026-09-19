using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
{
    public interface IEntityDamageSystem
    {
        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity initiatingCombatant, AbilityStage abilityStage, double tick);
    }
}