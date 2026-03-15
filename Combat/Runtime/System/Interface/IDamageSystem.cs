using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDamageSystem
    {
        public void ApplyDamage(byte targetInstanceID, StatCard attackerStats);
    }
}