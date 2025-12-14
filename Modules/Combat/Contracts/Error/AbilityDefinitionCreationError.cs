using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct AbilityDefinitionCreationError
    {
        public required AbilityDefinitionCreation[] AbilityDefinitionCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}