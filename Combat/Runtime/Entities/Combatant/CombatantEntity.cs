using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Contracts;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities.Combatant
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required Information CombatantInformation { get; init; }

        public CombatantEntity(StatCard statCard, AgilityCard agilityCard) 
            : base(BuildStatsComponent(statCard), BuildAgilityComponent(agilityCard))
        { 
            AddComponent(new LifeStatusComponent { IsAlive = true });
        }

        public void UpdateCombatantStats(StatsComponent statsComponent)
        { 
            RemoveComponent<StatsComponent>();
            AddComponent(statsComponent);
        }

        public void UpdateLifeStatus(bool isAlive)
        {
            RemoveComponent<LifeStatusComponent>();
            AddComponent(new LifeStatusComponent { IsAlive = isAlive });
        }

        private static StatsComponent BuildStatsComponent(StatCard statCardSource)
        {
            return new StatsComponent
            {
                Attack = statCardSource.Attack,
                Health = statCardSource.Health,
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
