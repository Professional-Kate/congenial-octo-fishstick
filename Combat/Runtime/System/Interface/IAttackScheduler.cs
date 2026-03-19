namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IAttackScheduler
    { 
        public void EnqueueInitial(double tick);

        public void EnqueueAttack(double tick, byte id);
    }
}