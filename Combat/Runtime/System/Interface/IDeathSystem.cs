using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IDeathSystem
    {
        public void KillEntity(CombatantEntity combatantEntity);
    }
}