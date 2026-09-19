using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Event
{
    public readonly record struct ScheduledCombatEvent
    {
        public required CombatEventType CombatEventType { get; init; }
        public required AbilityEntity AbilityEntity { get; init; }
        public required CombatantEntity CombatantEntity { get; init; }
        public required byte AbilityStageIndex { get; init; }
        public required double Tick { get; init; }
    }
}