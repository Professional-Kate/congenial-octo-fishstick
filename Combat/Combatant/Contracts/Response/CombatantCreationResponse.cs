using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Combatant.Contracts.Response
{
    public readonly record struct CombatantCreationResponse
    { 
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
        public required byte CombatantID { get; init; }
    }
}