using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Combatant.Model
{
    public readonly record struct CombatantDefinition
    {
        public required byte CombatantID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required HealthCard HealthCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
    }
}