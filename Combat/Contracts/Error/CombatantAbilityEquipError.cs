using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct CombatantAbilityEquipError
    {
        public required CombatantAbilityEquip[] CombatantAbilityEquips { get; init; }
        public required BaseError BaseError { get; init; }
    }
}