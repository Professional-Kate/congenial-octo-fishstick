using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Logging.Interface
{
    public interface IReadOnlyAbilityFactory
    {
        public ReadOnlyAbility[] Create(AbilityEntity[] abilityEntities);
    }
}