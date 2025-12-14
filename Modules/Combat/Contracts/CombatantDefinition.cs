using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantDefinition
    {
        public required CombatantType CombatantType { get; init; }
        public required CombatantStats CombatantStats { get; init; }
        public required Information Information { get; init; }
    }
}