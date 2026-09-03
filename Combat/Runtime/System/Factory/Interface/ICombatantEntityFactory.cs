using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ICombatantEntityFactory
    {
        public CombatantEntity[] Create(IReadOnlyList<CombatantDefinition> combatantDefinitions, TargetingType targetingType);
    }
}