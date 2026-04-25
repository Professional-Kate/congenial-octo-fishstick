using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface IAbilityEntityFactory
    {
        public AbilityEntity CreateAbilityEntity(AbilityCreation abilityCreation);
    }
}