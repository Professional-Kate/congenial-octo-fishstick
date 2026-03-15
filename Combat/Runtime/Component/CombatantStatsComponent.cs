using IdelPog.Combat.Contracts.Card;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct CombatantStatsComponent : IComponent
    { 
        public required StatCard StatCard { get; init; }
    }
}