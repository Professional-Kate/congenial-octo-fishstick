using IdelPog.Combat.Ability.Runtime.Entity;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IAbilityEffectValueCalculator
    {
        public void Calculate(AbilityEntity abilityEntity);
    }
}