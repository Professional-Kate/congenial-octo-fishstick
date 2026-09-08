using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Ability.Runtime.Entities
{
    public sealed record AbilityEntity : Entity
    {
        public required byte InstanceID { get; init; }
        public required byte AbilityID { get; init; }

        public AbilityEntity(StatsComponent statsComponent, TriggerComponent triggerComponent, AbilityStagesComponent abilityStagesComponent) 
            : base(triggerComponent, abilityStagesComponent, statsComponent)
        {
        }
        
        public uint GetStat(StatType statType) => GetComponent<StatsComponent>().GetStat(statType);
    }
}