using IdelPog.Combat.Contracts.Ability;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct CombatantAbilityCreation
    {
        public required Information Information { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required uint Speed { get; init; }
        public required uint Damage { get; init; }
    }
}