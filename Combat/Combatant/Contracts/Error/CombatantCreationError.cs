using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Combatant.Contracts.Error
{
    public readonly record struct CombatantCreationError
    {
        public required CombatantCreation[] CombatantCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}