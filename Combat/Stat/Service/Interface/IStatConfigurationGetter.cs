using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Service.Interface
{
    public interface IStatConfigurationGetter
    { 
        public byte GetStatID(StatType statType);
    }
}