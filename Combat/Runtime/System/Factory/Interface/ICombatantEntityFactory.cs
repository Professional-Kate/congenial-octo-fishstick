using IdelPog.Combat.Contracts.Card.Combatant;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ICombatantEntityFactory
    { 
        public void SpawnCombatants(IReadOnlyList<CombatantCard> combatants, bool isFriendly);
    }
}