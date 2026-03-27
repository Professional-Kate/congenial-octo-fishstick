using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatantFactory
    { 
        public void SpawnCombatants(IReadOnlyList<CombatantCard> combatants, bool isFriendly);
    }
}