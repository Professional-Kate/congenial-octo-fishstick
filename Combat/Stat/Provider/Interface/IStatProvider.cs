using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Provider.Interface
{
    public interface IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity, StatType statType);
    }
}