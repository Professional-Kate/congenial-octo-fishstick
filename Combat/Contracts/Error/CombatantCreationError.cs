using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct CombatantCreationError
    {
        public required CombatantCreation[] CombatantCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}