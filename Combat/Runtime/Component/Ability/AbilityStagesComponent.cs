using System.Collections.Immutable;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct AbilityStagesComponent : IComponent
    { 
        public required ImmutableArray<CombatantAbilityStage> AbilityStages { get; init; }
    }
}