using IdelPog.Combat.Ability.Runtime.Entities;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IAbilityEffectValueCalculator
    {
        public void Calculate(AbilityEntity abilityEntity);
    }
}