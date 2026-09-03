using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Core.Filter.Provider.Interface
{
    public interface IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity);
    }
}