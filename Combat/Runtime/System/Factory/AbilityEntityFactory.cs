using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Repository.Asserter;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class AbilityEntityFactory : IAbilityEntityFactory
    {
        private readonly IRepositoryAsserter _repositoryAsserter;

        public AbilityEntityFactory(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public AbilityEntity CreateAbilityEntity(AbilityCreation abilityCreation)
        {
            SpeedComponent speedComponent = new() { Speed = abilityCreation.Speed };
            DamageComponent damageComponent = new() { Damage = abilityCreation.Damage };

            return new AbilityEntity(_repositoryAsserter, speedComponent, damageComponent)
            {
                AbilityType = abilityCreation.AbilityType,
                Information = abilityCreation.Information
            };
        }
    }
}