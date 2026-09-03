using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IReadyTickSystem
    {
        public void SetNextReadyTick(double currentTick, AbilityEntity abilityEntity, uint combatantSpeed);
    }
}