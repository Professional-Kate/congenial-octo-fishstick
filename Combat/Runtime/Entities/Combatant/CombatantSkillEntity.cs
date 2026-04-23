using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities.Combatant
{
    public sealed record CombatantSkillEntity : Entity
    {
        public required byte CombatantID { get; init; }
        
        public CombatantSkillEntity(IRepositoryAsserter repositoryAsserter) 
            : base(repositoryAsserter)
        {
        }
    }
}