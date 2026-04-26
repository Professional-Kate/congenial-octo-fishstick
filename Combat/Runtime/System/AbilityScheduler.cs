using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AbilityScheduler : IAbilityScheduler
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAssetRepository<AbilityType, AbilityEntity> _abilityEntityRepository;
        
        public void RegisterInitial(byte startingTick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            {
                
            }
        }
    }
}