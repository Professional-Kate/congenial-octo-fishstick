using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Ability.Runtime.Entities
{
    public sealed record AbilityEntity : Entity
    {
        public required byte InstanceID { get; init; }
        public required byte AbilityID { get; init; }
        
        private readonly IStatConfigurationGetter _statConfigurationGetter;

        public AbilityEntity(IStatConfigurationGetter statConfigurationGetter, StatsComponent statsComponent, TriggerComponent triggerComponent, AbilityStagesComponent abilityStagesComponent) 
            : base(triggerComponent, abilityStagesComponent, statsComponent)
        {
            _statConfigurationGetter = statConfigurationGetter;
        }
        
        public uint GetStat(StatType statType) => GetComponent<StatsComponent>().GetStat(_statConfigurationGetter.GetStatID(statType));
    }
}