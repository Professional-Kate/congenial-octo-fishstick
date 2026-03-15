namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct CombatantCard : ICard
    { 
        public required CombatantType CombatantType { get; init; }
        public required StatCard StatCard { get; init; }
    }
}