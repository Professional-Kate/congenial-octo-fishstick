using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IBasicAttackScheduler
    { 
        public void EnqueueInitial(double tick);

        public void EnqueueAttack(double tick, byte attackerID, AbilityType abilityType);
    }
}