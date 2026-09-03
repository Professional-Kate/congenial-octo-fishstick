using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Combatant.Runtime.Entities
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required byte InstanceID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required TargetingType TargetingType { get; init; }

        public CombatantEntity(StatCard statCard, AgilityCard agilityCard) 
            : base(BuildStatsComponent(statCard), BuildAgilityComponent(agilityCard))
        { 
            AddComponent(new LifeStatusComponent { IsAlive = true });
            AddComponent(new BaseHealthComponent { Health = statCard.Health });
        }

        private static HealthComponent BuildStatsComponent(StatCard statCardSource)
        {
            return new HealthComponent
            {
                Health = statCardSource.Health
            };
        }
        
        private static AgilityComponent BuildAgilityComponent(AgilityCard agilityCardSource)
        {
            return new AgilityComponent
            {
                Speed = agilityCardSource.Speed,
                Initiative = agilityCardSource.Initiative
            };
        }
    }
}
