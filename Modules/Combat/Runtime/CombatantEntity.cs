using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime
{
    public sealed record class CombatantEntity : Entity
    {
        public readonly CombatantType CombatantType;
        
        public CombatantEntity(CombatantType combatantType, IRepositoryAsserter repositoryAsserter, OffensiveStatsComponent offensiveStatsComponent, DefensiveStatsComponent defensiveStatsComponent, UtilityStatsComponent utilityStatsComponent) 
            : base(repositoryAsserter, offensiveStatsComponent, defensiveStatsComponent, utilityStatsComponent)
        { 
            CombatantType = combatantType;
        }
    }
}