using IdelPog.Combat.Runtime;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatLog
    {
        public void Append(CombatantEntity defendingCombatant, CombatantEntity attackingCombatant);

        public ICombatLogReader EndCombat();
    }
}