using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface ICombatantAbilityEntityRepository
    {
        public void Add(byte combatantID, IReadOnlyList<CombatantAbilityEntity> combatantAbilities);
        
        public CombatantAbilityEntity GetAbilityEntity(byte combatantID, AbilityType abilityType);
        
        public IReadOnlyList<CombatantAbilityEntity> GetAll(byte combatantID);
    }
}