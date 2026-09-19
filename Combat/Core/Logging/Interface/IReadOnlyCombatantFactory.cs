using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Logging.Interface
{
    public interface IReadOnlyCombatantFactory
    {
        public ReadOnlyCombatant[] Create(CombatantEntity[] combatantEntities);
    }
}