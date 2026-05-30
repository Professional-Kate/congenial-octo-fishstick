using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct AttackingCombatant
    {
        public required byte CombatantID { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required uint DamageDealt { get; init; }
    }
}