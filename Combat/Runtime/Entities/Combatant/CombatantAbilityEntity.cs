using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities.Combatant
{
    public sealed record CombatantAbilityEntity : Entity
    {
        public required byte CombatantID { get; init; }
        
        public CombatantAbilityEntity(IRepositoryAsserter repositoryAsserter) 
            : base(repositoryAsserter)
        {
        }
    }
}