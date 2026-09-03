using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;

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
                combatantEntities[i] = new CombatantEntity(combatantDefinition.StatCard, combatantDefinition.AgilityCard)
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
    }
}