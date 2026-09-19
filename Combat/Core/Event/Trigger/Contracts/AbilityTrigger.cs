using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct AbilityTrigger
    {
        public required double Tick { get; init; }
        public required AbilityEntity AbilityEntity { get; init; }
        public required CombatantEntity CombatantEntity { get; init; }
    }
}