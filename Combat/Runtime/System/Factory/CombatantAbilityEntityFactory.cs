using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantAbilityEntityFactory : ICombatantAbilityEntityFactory
    {
        private readonly IAssetRepository<AbilityType, AbilityEntity> _skillEntityRepository;
        private readonly IRepositoryAsserter _repositoryAsserter;
        private readonly IFoundAssertion _foundAssertion;

        public CombatantAbilityEntityFactory(IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository, IRepositoryAsserter repositoryAsserter, IFoundAssertion foundAssertion)
        {
            _skillEntityRepository = skillEntityRepository;
            _repositoryAsserter = repositoryAsserter;
            _foundAssertion = foundAssertion;
        }

        public IReadOnlyList<CombatantAbilityEntity> Create(CombatantAbilityEquip combatantAbilityEquip)
        {
            CombatantAbilityEntity[] combatantAbilityEntities = new CombatantAbilityEntity[combatantAbilityEquip.AbilityCards.Length];
            for (int i = 0; i < combatantAbilityEquip.AbilityCards.Length; i++)
            {
                AbilityCard abilityCard = combatantAbilityEquip.AbilityCards[i];
                _foundAssertion.AssertFound(abilityCard.AbilityType, _skillEntityRepository.Contains(abilityCard.AbilityType));

                AbilityEntity abilityEntity = _skillEntityRepository.Get(abilityCard.AbilityType);
                CombatantAbilityEntity combatantAbilityEntity = new(_repositoryAsserter) { CombatantID = combatantAbilityEquip.CombatantID, AbilityType = abilityCard.AbilityType };
                AddBaseComponents(combatantAbilityEntity, abilityEntity, abilityCard.StrategyCard.TargetingType);
                
                combatantAbilityEntities[i] = combatantAbilityEntity;
            }
            
            return combatantAbilityEntities;
        }

        private static void AddBaseComponents(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity, TargetingType targetingType)
        {
            combatantAbilityEntity.AddComponent(new TargetingTypeComponent { TargetingType = targetingType });
            combatantAbilityEntity.AddComponent(abilityEntity.GetComponent<CooldownComponent>());
            combatantAbilityEntity.AddComponent(abilityEntity.GetComponent<DamageComponent>());
        }
    }
}