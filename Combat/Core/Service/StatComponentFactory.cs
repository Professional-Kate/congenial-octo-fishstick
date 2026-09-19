using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Core.Service
{
    public sealed class StatComponentFactory : IStatComponentFactory
    {
        private readonly IStatConfigurationGetter _statConfigurationGetter;
        private readonly IIncrementalRepository<StatCreation> _statCreationRepository;

        public StatComponentFactory(IStatConfigurationGetter statConfigurationGetter, IIncrementalRepository<StatCreation> statCreationRepository)
        {
            _statConfigurationGetter = statConfigurationGetter;
            _statCreationRepository = statCreationRepository;
        }

        public StatComponent Create(StatType statType, uint baseStat)
        {
            (byte statID, StatCreation statCreation) = GetStatCreation(statType);

            return new StatComponent { StatID = statID, Value = GetFinalValue(baseStat, statCreation.InitialValue) };
        }

        public uint CalculateStat(StatType statType, uint baseStat)
        {
            (byte _, StatCreation statCreation) = GetStatCreation(statType);
            
            return GetFinalValue(baseStat, statCreation.InitialValue);
        }
        
        private (byte statID, StatCreation statCreation) GetStatCreation(StatType statType)
        {
            // TODO: no longer apply the initial stat here. That will be in the Mutator pipeline
            byte statID = _statConfigurationGetter.GetStatID(statType);
            StatCreation statCreation = _statCreationRepository.Get(statID);
            
            return (statID, statCreation);
        }

        private static uint GetFinalValue(uint baseStat, uint initialValue) => baseStat + initialValue;
    }
}