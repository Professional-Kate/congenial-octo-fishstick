using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts.Ability
{
    public readonly record struct CombatantAbility
    {
        public required AbilityType AbilityType { get; init; }
        public required double Cooldown { get; init; }
        public required DamageCard DamageCard { get; init; }
    }
}