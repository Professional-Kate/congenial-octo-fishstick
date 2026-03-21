using IdelPog.Combat.Contracts;

namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStoreGet
    {
        public LowestHealthCombatant LowestHealthCombatant { get; }
        public HighestAttackCombatant HighestAttackCombatant { get; }
    }
}