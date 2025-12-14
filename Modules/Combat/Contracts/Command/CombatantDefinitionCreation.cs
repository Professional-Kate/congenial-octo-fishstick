using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct CombatantDefinitionCreation
    {
        public required CombatantType CombatantType { get; init; }
        public required CombatantStats CombatantStats { get; init; }
        public required Information Information { get; init; }
    }
}