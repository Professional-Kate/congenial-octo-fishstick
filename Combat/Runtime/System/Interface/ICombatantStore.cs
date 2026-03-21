using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatantStore
    {
        public LowestHealthCombatant LowestHealthCombatant { get; }
        public HighestAttackCombatant HighestAttackCombatant { get; }

        public void RegisterInitial(IEnumerable<CombatantEntity> combatants);
        
        public void RegisterCombatantChange(byte combatantID, StatCard statCard);

        public void RegisterCombatantDeath(byte combatantID, IEnumerable<CombatantEntity> combatants);
    }
}