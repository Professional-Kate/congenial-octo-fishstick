using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct CombatantDefinitionCreationError
    {
        public required CombatantDefinitionCreation[] CombatantDefinitionsCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}