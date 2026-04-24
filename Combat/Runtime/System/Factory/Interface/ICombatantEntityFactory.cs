using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ICombatantEntityFactory
    {
        public CombatantEntity CreateEntity(CombatantCreation combatantCreation);
        
        public void SpawnCombatants(IReadOnlyList<CombatantCreation> combatants, bool isFriendly);
    }
}