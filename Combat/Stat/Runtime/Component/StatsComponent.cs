using System.Collections.Immutable;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Stat.Runtime.Component
{
    public readonly record struct StatsComponent : IComponent
    {
        public required StatComponent[] StatComponents { private get; init; }

        public uint GetStat(StatType statType)
        {
            foreach (StatComponent statComponent in StatComponents)
            {
                if (statComponent.StatType == statType)
                {
                    return statComponent.Stat;
                }
            }
            
            throw new KeyNotFoundException();
        }

        public void ReplaceStat(StatType statType, uint newStat)
        {
            for (int i = 0; i < StatComponents.Length; i++)
            {
                StatComponent statComponent = StatComponents[i];
                if (statComponent.StatType != statType)
                {
                    continue;
                }

                StatComponents[i] = statComponent with { Stat = newStat };
                return;
            }
            
            throw new KeyNotFoundException();
        }
        
        public ImmutableArray<StatComponent> GetAllStats() => [..StatComponents];
    }
}