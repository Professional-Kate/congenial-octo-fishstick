using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;

namespace IdelPog.Combat.Combatant.Runtime
{
    public sealed class CombatantEntityFactory : ICombatantEntityFactory
    {
        private readonly IStatConfigurationGetter _statConfigurationGetter;
        private readonly IStatComponentFactory _statComponentFactory;
        
        private byte _instanceID;

        public CombatantEntityFactory(IStatConfigurationGetter statConfigurationGetter, IStatComponentFactory statComponentFactory)
        {
            _statConfigurationGetter = statConfigurationGetter;
            _statComponentFactory = statComponentFactory;
        }

        public CombatantEntity[] Create(IReadOnlyList<CombatantDefinition> combatantDefinitions, TargetingType targetingType)
        {
            CombatantEntity[] combatantEntities = new CombatantEntity[combatantDefinitions.Count];
            for (int i = 0; i < combatantDefinitions.Count; i++)
            {
                CombatantDefinition combatantDefinition = combatantDefinitions[i];

                StatsComponent statsComponent = new() { StatComponents = CreateStatComponents(combatantDefinition) };
                combatantEntities[i] = new CombatantEntity(statsComponent, _statConfigurationGetter)
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

        private StatComponent[] CreateStatComponents(CombatantDefinition combatantDefinition)
        {
            return
            [
                _statComponentFactory.Create(StatType.BASE_HEALTH, combatantDefinition.HealthCard.BaseHealth),
                _statComponentFactory.Create(StatType.HEALTH, combatantDefinition.HealthCard.Health),
                _statComponentFactory.Create(StatType.SPEED, combatantDefinition.AgilityCard.Speed),
                _statComponentFactory.Create(StatType.INITIATIVE, combatantDefinition.AgilityCard.Initiative)
            ];
        }
    }
}