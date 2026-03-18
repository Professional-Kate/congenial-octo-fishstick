using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record CombatantEntity : Entity
    {
        public required bool IsFriendly { get; init; }

        public CombatantEntity(IRepositoryAsserter repositoryAsserter, StatCard statCard) 
            : base(repositoryAsserter, new CombatantStatsComponent { StatCard = statCard })
        { 
        }

        public void UpdateCombatantStats(StatCard statCard)
        { 
            RemoveComponent<CombatantStatsComponent>();
            AddComponent(new CombatantStatsComponent { StatCard = statCard });
        }
    }
}
