using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ITargetFinder
    {
        public CombatantEntity FindBestTarget(CombatantEntity instigatingEntity, AbilityType abilityType);
    }
}