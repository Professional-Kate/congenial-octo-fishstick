using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantAbility
    {
        public required AbilityType AbilityType { get; init; }
        public required double Cooldown { get; init; }
        public required ElementalDamageCard ElementalDamageCard { get; init; }
        public required PhysicalDamageCard PhysicalDamageCard { get; init; }
    }
}