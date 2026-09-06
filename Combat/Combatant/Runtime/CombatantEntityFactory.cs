using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Combatant.Runtime
{
    public sealed class CombatantEntityFactory : ICombatantEntityFactory
    {
        private byte _instanceID;
        
        public CombatantEntity[] Create(IReadOnlyList<CombatantDefinition> combatantDefinitions, TargetingType targetingType)
        {
            CombatantEntity[] combatantEntities = new CombatantEntity[combatantDefinitions.Count];
            for (int i = 0; i < combatantDefinitions.Count; i++)
            {
                CombatantDefinition combatantDefinition = combatantDefinitions[i];

                StatsComponent statsComponent = new() { StatComponents = CreateStatComponents(combatantDefinition) };
                combatantEntities[i] = new CombatantEntity(statsComponent)
                {
                    InstanceID = _instanceID,
                    CombatantID = combatantDefinition.CombatantID,
                    CombatantType = combatantDefinition.CombatantType,
                    TargetingType = targetingType
                };

                checked
                { 
                    _instanceID++;
                }
            }
            
            return combatantEntities;
        }

        private static StatComponent[] CreateStatComponents(CombatantDefinition combatantDefinition)
        {
            return
            [
                new StatComponent { StatType = StatType.BASE_HEALTH, Stat = combatantDefinition.HealthCard.BaseHealth },
                new StatComponent { StatType = StatType.HEALTH, Stat = combatantDefinition.HealthCard.Health },
                new StatComponent { StatType = StatType.SPEED, Stat = combatantDefinition.AgilityCard.Speed },
                new StatComponent { StatType = StatType.INITIATIVE, Stat = combatantDefinition.AgilityCard.Initiative }
            ];
        }
    }
}