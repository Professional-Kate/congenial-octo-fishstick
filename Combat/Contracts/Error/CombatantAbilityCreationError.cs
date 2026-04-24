using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct CombatantAbilityCreationError
    {
        public required CombatantAbilityCreation[] CombatantAbilityCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}