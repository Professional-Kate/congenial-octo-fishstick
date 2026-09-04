using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Combatant.Contracts.Command
{
    public readonly record struct CombatantCreation
    { 
        public required CombatantType CombatantType { get; init; }
        public required HealthCard HealthCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
    }
}