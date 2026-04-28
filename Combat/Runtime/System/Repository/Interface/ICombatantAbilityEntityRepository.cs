using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface ICombatantAbilityEntityRepository
    {
        public void Add(byte combatantID, IReadOnlyList<CombatantAbilityEntity> combatantAbilities);
        
        public bool Contains(byte combatantID);
        
        public CombatantAbilityEntity Get(byte combatantID, AbilityType abilityType);
        
        public IReadOnlyList<CombatantAbilityEntity> GetAll(byte combatantID);
    }
}