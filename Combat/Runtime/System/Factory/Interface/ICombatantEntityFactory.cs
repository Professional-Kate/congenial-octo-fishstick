using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ICombatantEntityFactory
    {
        public CombatantEntity[] Create(IReadOnlyList<CombatantDefinition> combatantDefinitions, TargetingType targetingType);
    }
}