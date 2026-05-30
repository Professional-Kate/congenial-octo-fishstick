using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.Filter.Provider.Interface
{
    public interface IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity);
    }
}