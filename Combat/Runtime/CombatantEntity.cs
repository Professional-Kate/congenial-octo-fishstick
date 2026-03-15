using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record CombatantEntity : Entity
    {
        public readonly byte InstanceID;

        public CombatantEntity(RepositoryAsserter repositoryAsserter, StatCard statCard, byte InstanceID) 
            : base(repositoryAsserter, new CombatantStatsComponent { StatCard = statCard })
        { 
            this.InstanceID = InstanceID;
        }

        public void UpdateCombatantStats(StatCard statCard)
        { 
            RemoveComponent<CombatantStatsComponent>();
            AddComponent(new CombatantStatsComponent { StatCard = statCard });
        }
    }
}
