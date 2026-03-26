using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public readonly bool IsFriendly;

        public CombatantEntity(IRepositoryAsserter repositoryAsserter, CombatantCard combatantCard) 
            : base(repositoryAsserter, BuildCombatantStatsComponent(combatantCard.StatCard), new TargetingTypeComponent { TargetingType = combatantCard.TargetingType }, new LifeStatusComponent { IsAlive = true })
        { 
            IsFriendly = combatantCard.IsFriendly;
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
