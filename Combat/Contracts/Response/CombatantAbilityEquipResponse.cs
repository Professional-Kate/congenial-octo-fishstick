using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct CombatantAbilityEquipResponse
    {
        public required byte CombatantID { get; init; }
        public required CombatantAbility[] CombatantAbilities { get; init; }
    }
}