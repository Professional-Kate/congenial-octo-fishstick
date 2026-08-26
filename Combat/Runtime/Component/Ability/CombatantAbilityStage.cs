namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct CombatantAbilityStage
    {
        public required AbilityStage AbilityStage { get; init; }
        public required TargetingPreferenceComponent TargetingPreferenceComponent { get; init; }
    }
}