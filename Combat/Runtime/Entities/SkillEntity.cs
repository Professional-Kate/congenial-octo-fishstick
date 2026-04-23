using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities
{
    public sealed record SkillEntity : Entity
    {
        public required SkillType SkillType { get; init; }
        public required Information Information { get; init; }
        
        public SkillEntity(IRepositoryAsserter repositoryAsserter, SpeedComponent speedComponent, DamageComponent damageComponent) 
            : base(repositoryAsserter, speedComponent, damageComponent)
        {
        }
    }
}