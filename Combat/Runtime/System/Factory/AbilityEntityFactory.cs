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
            CooldownComponent cooldownComponent = new() { Cooldown = abilityCreation.Cooldown };
            DamageComponent damageComponent = new() { Damage = abilityCreation.Damage };

            return new AbilityEntity(_repositoryAsserter, cooldownComponent, damageComponent)
            {
                AbilityType = abilityCreation.AbilityType,
                AbilitySlots = abilityCreation.AbilitySlots,
                Information = abilityCreation.Information
            };
        }
    }
}