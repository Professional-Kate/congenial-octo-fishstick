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

        public CombatantEntity(StatCard statCard) 
            : base(BuildCombatantStatsComponent(statCard))
        { 
            AddComponent(new LifeStatusComponent { IsAlive = true });
        }

        public void UpdateCombatantStats(CombatantStatsComponent combatantStatsComponent)
        { 
            RemoveComponent<CombatantStatsComponent>();
            AddComponent(combatantStatsComponent);
        }

        public void UpdateLifeStatus(bool isAlive)
        {
            RemoveComponent<LifeStatusComponent>();
            AddComponent(new LifeStatusComponent { IsAlive = isAlive });
        }

        private static CombatantStatsComponent BuildCombatantStatsComponent(StatCard statCardSource)
        {
            return new CombatantStatsComponent
            {
                Attack = statCardSource.Attack,
                Health = statCardSource.Health,
                Speed = statCardSource.Speed
            };
        }
    }
}
