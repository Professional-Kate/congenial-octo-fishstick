using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Combatant.Runtime.Entities
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required byte InstanceID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required TargetingType TargetingType { get; init; }

        private readonly IStatConfigurationGetter _statConfigurationGetter;
        
        public CombatantEntity(StatsComponent statsComponent, IStatConfigurationGetter statConfigurationGetter) 
            : base(requiredComponents: [statsComponent, new LifeStatusComponent { IsAlive = true }])
        {
            _statConfigurationGetter = statConfigurationGetter;
        }

        public uint GetStat(StatType statType) => GetComponent<StatsComponent>().GetStat(_statConfigurationGetter.GetStatID(statType));
        
        public void ReplaceStat(StatType statType, uint newStat) => GetComponent<StatsComponent>().ReplaceStat(_statConfigurationGetter.GetStatID(statType), newStat);
    }
}
