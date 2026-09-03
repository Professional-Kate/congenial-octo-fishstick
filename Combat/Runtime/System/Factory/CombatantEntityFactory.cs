using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
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