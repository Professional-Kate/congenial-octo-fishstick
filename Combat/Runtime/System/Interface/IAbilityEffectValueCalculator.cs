using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IAbilityEffectValueCalculator
    {
        public void Calculate(AbilityEntity abilityEntity);
    }
}