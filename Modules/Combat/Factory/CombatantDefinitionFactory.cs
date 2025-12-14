using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantDefinitionFactory : ICombatantDefinitionFactory
    {
        public CombatantDefinition Create(CombatantDefinitionCreation combatantDefinitionCreation)
        {
            return new CombatantDefinition
            {
                CombatantType = combatantDefinitionCreation.CombatantType,
                CombatantStats = combatantDefinitionCreation.CombatantStats,
                Information = combatantDefinitionCreation.Information
            };
        }
    }
}