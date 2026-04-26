using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantAbility
    {
        public required AbilityType AbilityType { get; init; }
        public required double Cooldown { get; init; }
        public required uint Damage { get; init; }
    }
}