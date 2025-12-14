using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct AbilityDefinitionCreationResponse
    {
        public required AbilityType AbilityType { get; init; }
        public required TargetingInformation TargetingInformation { get; init; }
        public required Information Information { get; init; }
        public required byte Cooldown { get; init; } 
        public required uint Damage { get; init; }
    }
}