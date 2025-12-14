using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Factory.Interface
{
    public interface ICombatantDefinitionFactory
    { 
        public CombatantDefinition Create(CombatantDefinitionCreation combatantDefinitionCreation);
    }
}