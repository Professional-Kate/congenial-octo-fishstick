using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
{
    public interface IDeathSystem
    {
        public void KillEntity(CombatantEntity combatantEntity);
    }
}