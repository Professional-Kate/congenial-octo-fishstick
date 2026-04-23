using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required bool IsFriendly { get; init; }
        public readonly CombatantType CombatantType;
        public readonly Information CombatantInformation;

        public CombatantEntity(IRepositoryAsserter repositoryAsserter, CombatantCard combatantCard) 
            : base(repositoryAsserter, BuildCombatantStatsComponent(combatantCard.StatCard), new TargetingTypeComponent { TargetingType = combatantCard.TargetingType }, new LifeStatusComponent { IsAlive = true })
        { 
            CombatantType = combatantCard.CombatantType;
            CombatantInformation = combatantCard.Information;
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
