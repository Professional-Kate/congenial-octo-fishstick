using System.Collections.Immutable;
using IdelPog.Combat.Stat.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;

namespace IdelPog.Combat.Stat.Service.Interface
{
    public interface IStatConfigurationSetter
    { 
        public void SetStatConfiguration(StatConfiguration statConfiguration);
        
        public ImmutableArray<StatBinding> GetStatBindings();
    }
}