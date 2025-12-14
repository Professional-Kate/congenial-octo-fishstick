using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct CombatantDefinitionCreationResponse
    {
        public required CombatantType CombatantType { get; init; }
        public required CombatantStats CombatantStats { get; init; }
        public required Information Information { get; init; }
    }
}