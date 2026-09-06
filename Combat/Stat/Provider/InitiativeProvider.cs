using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Provider.Interface;

namespace IdelPog.Combat.Stat.Provider
{
    public sealed class InitiativeProvider : IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity)
        { 
            return combatantEntity.GetStat(StatType.INITIATIVE);
        }
    }
}