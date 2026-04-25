using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct AbilityCreationError
    {
        public required AbilityCreation[] AbilityCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}