using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Runtime.Filter.Provider.Interface
{
    public interface IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity);
    }
}