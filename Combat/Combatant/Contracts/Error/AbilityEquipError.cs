using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Combatant.Contracts.Error
{
    public readonly record struct AbilityEquipError
    {
        public required AbilityEquip[] AbilityEquips { get; init; }
        public required BaseError BaseError { get; init; }
    }
}