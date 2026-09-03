using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
{
    public interface ICombatantEntityFactory
    {
        public CombatantEntity[] Create(IReadOnlyList<CombatantDefinition> combatantDefinitions, TargetingType targetingType);
    }
}