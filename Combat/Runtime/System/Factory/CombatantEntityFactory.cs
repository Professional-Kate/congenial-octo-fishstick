using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantEntityFactory : ICombatantEntityFactory
    {
        public CombatantEntity CreateEntity(CombatantCreation combatantCreation, byte combatantID)
        {
            return new CombatantEntity(combatantCreation.StatCard)
            {
                CombatantID = combatantID,
                CombatantType =  combatantCreation.CombatantType,
                CombatantInformation = combatantCreation.Information
            };
        }
    }
}