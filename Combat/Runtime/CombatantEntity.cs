using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record CombatantEntity : Entity
    {
        public CombatantEntity(RepositoryAsserter repositoryAsserter, StatCard statCard) 
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
