using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Logging.Interface
{
    public interface IInitialStateLogger
    {
        public InitialEntities LogInitialState(CombatantEntity[] friendlyCombatants, CombatantEntity[] enemyCombatants, AbilityEntity[] abilityEntities);
    }
}