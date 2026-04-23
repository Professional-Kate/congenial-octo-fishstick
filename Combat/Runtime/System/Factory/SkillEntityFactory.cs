using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Repository.Asserter;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class SkillEntityFactory : ISkillEntityFactory
    {
        private readonly IRepositoryAsserter _repositoryAsserter;

        public SkillEntityFactory(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public SkillEntity CreateSkillEntity(CombatantSkillCreation combatantSkillCreation)
        {
            SpeedComponent speedComponent = new() { Speed = combatantSkillCreation.Speed };
            DamageComponent damageComponent = new() { Damage = combatantSkillCreation.Damage };

            return new SkillEntity(_repositoryAsserter, speedComponent, damageComponent)
            {
                SkillType = combatantSkillCreation.SkillType,
                Information = combatantSkillCreation.Information
            };
        }
    }
}