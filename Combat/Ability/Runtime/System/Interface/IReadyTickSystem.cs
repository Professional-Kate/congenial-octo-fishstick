using IdelPog.Combat.Ability.Runtime.Entities;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface IReadyTickSystem
    {
        public void SetNextReadyTick(double currentTick, AbilityEntity abilityEntity, uint combatantSpeed);
    }
}