using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct CombatantSkillCreationError
    {
        public required CombatantSkillCreation[] CombatantSkillCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}