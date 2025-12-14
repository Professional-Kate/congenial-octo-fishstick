using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct AbilityDefinitionCreation
    {
        public required AbilityType AbilityType { get; init; }
        public required TargetingInformation TargetingInformation { get; init; }
        public required Information Information { get; init; }
        public required byte Cooldown { get; init; } 
        public required uint Damage { get; init; }
    }
}