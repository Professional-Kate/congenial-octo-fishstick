using IdelPog.Combat.Contracts;

namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStoreRead
    {
        public LowestHealthCombatant LowestHealthCombatant { get; }
        public HighestAttackCombatant HighestAttackCombatant { get; }
    }
}