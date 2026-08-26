using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantAbilityFactory : ICombatantAbilityFactory
    {
        public byte[] GetCombatantAbilityIDs(IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        {
            byte[] combatantAbilityIDs = new byte[combatantAbilityEntities.Count];
            for (int i = 0; i < combatantAbilityEntities.Count; i++)
            {
                combatantAbilityIDs[i] = combatantAbilityEntities[i].AbilityID;
            }

            return combatantAbilityIDs;
        }
    }
}