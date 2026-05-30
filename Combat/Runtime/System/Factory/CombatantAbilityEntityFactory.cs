using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantAbilityEntityFactory : ICombatantAbilityEntityFactory
    {
        private readonly IAssetRepository<AbilityType, AbilityEntity> _skillEntityRepository;

        public CombatantAbilityEntityFactory(IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository)
        {
            _skillEntityRepository = skillEntityRepository;
        }

        public IReadOnlyList<CombatantAbilityEntity> Create(CombatantAbilityEquip combatantAbilityEquip)
        {
            CombatantAbilityEntity[] combatantAbilityEntities = new CombatantAbilityEntity[combatantAbilityEquip.AbilityCards.Length];
            for (int i = 0; i < combatantAbilityEquip.AbilityCards.Length; i++)
            {
                AbilityCard abilityCard = combatantAbilityEquip.AbilityCards[i];

                AbilityEntity abilityEntity = _skillEntityRepository.Get(abilityCard.AbilityType);
                CombatantAbilityEntity combatantAbilityEntity = new() { CombatantID = combatantAbilityEquip.CombatantID, AbilityType = abilityCard.AbilityType };
                AddBaseComponents(combatantAbilityEntity, abilityEntity, abilityCard.StrategyCard.TargetingPreference, abilityCard.StrategyCard.CombatantStatType);
                
                combatantAbilityEntities[i] = combatantAbilityEntity;
            }
            
            return combatantAbilityEntities;
        }

        private static void AddBaseComponents(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity, TargetingPreference targetingPreference, CombatantStatType combatantStatType)
        {
            combatantAbilityEntity.AddComponent(new TargetingPreferenceComponent { TargetingPreference = targetingPreference, CombatantStatType = combatantStatType});
            combatantAbilityEntity.AddComponent(abilityEntity.GetComponent<CooldownComponent>());
            combatantAbilityEntity.AddComponent(abilityEntity.GetComponent<ElementalDamageComponent>());
            combatantAbilityEntity.AddComponent(abilityEntity.GetComponent<PhysicalDamageComponent>());

            if (abilityEntity.TryGetComponent(out CastTimeComponent castTimeComponent))
            { 
                combatantAbilityEntity.AddComponent(castTimeComponent);
            }
        }
    }
}