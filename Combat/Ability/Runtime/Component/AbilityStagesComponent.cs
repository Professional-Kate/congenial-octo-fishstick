using System.Collections.Immutable;
using IdelPog.Combat.Ability.Model;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct AbilityStagesComponent : IComponent
    { 
        public required ImmutableArray<AbilityStage> AbilityStages { get; init; }
    }
}