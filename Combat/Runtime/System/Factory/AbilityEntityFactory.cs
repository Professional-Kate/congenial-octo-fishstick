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

        public AbilityEntity CreateAbilityEntity(CombatantAbilityCreation combatantAbilityCreation)
        {
            SpeedComponent speedComponent = new() { Speed = combatantAbilityCreation.Speed };
            DamageComponent damageComponent = new() { Damage = combatantAbilityCreation.Damage };

            return new AbilityEntity(_repositoryAsserter, speedComponent, damageComponent)
            {
                AbilityType = combatantAbilityCreation.AbilityType,
                Information = combatantAbilityCreation.Information
            };
        }
    }
}