using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct CombatantCreationResponse
    { 
        public required Information Information { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
    }
}