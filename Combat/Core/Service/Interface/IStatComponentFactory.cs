using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Core.Service.Interface
{
    public interface IStatComponentFactory
    {
        public StatComponent Create(StatType statType, uint baseStat);

        public uint CalculateStat(StatType statType, uint baseStat);
    }
}