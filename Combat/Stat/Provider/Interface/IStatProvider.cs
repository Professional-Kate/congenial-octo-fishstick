using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Stat.Provider.Interface
{
    public interface IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity);
    }
}