using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Ability.Contracts.Error
{
    public readonly record struct AbilityEquipError
    {
        public required AbilityEquip[] AbilityEquips { get; init; }
        public required BaseError BaseError { get; init; }
    }
}