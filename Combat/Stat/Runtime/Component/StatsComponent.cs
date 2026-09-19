using System.Collections.Immutable;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Stat.Runtime.Component
{
    public readonly record struct StatsComponent : IComponent
    {
        public required StatComponent[] StatComponents { private get; init; }

        public uint GetStat(byte statID)
        {
            foreach (StatComponent statComponent in StatComponents)
            {
                if (statComponent.StatID == statID)
                {
                    return statComponent.Value;
                }
            }
            
            throw new KeyNotFoundException();
        }

        public void ReplaceStat(byte statID, uint newStat)
        {
            for (int i = 0; i < StatComponents.Length; i++)
            {
                StatComponent statComponent = StatComponents[i];
                if (statComponent.StatID != statID)
                {
                    continue;
                }

                StatComponents[i] = statComponent with { Value = newStat };
                return;
            }
            
            throw new KeyNotFoundException();
        }
        
        public ImmutableArray<StatComponent> GetAllStats() => [..StatComponents];
    }
}