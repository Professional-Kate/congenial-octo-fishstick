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
            : base(repositoryAsserter, new CombatantStatsComponent { StatCard = combatantCard.StatCard }, new TargetingTypeComponent { TargetingType = combatantCard.TargetingType })
        { 
            IsFriendly = combatantCard.IsFriendly;
            AddComponent(new LifeStatusComponent { IsAlive = true });
        }

        public void UpdateCombatantStats(StatCard statCard)
        { 
            RemoveComponent<CombatantStatsComponent>();
            AddComponent(new CombatantStatsComponent { StatCard = statCard });
        }

        public void UpdateLifeStatus(bool isAlive)
        {
            RemoveComponent<LifeStatusComponent>();
            AddComponent(new LifeStatusComponent { IsAlive = isAlive });
        }
    }
}
