using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDeathSystem
    {
        public void KillEntity(CombatantEntity combatantEntity);
    }
}