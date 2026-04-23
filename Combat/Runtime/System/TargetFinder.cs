using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TargetFinder : ITargetFinder
    {
        private readonly ICombatantStoreRead _friendlyCombatantStore;
        private readonly ICombatantStoreRead _enemyCombatantStore;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public TargetFinder(ICombatantStoreRead friendlyCombatantStore, ICombatantStoreRead enemyCombatantStore, ICombatantRepository combatantRepository, IObjectNullAssertion objectNullAssertion, IFoundAssertion foundAssertion)
        {
            _friendlyCombatantStore = friendlyCombatantStore;
            _enemyCombatantStore = enemyCombatantStore;
            _combatantRepository = combatantRepository;
            _objectNullAssertion = objectNullAssertion;
            _foundAssertion = foundAssertion;
        }

        public CombatantEntity FindBestTarget(CombatantEntity attackingEntity, SkillType skillType)
        {
            return DetermineTarget(attackingEntity.IsFriendly ? _enemyCombatantStore : _friendlyCombatantStore, attackingEntity, skillType);
        }

        private CombatantEntity DetermineTarget(ICombatantStoreRead combatantStore, CombatantEntity attackingEntity, SkillType skillType)
        {
            TargetingType targetingType = GetTargetingType(attackingEntity, skillType);
            CombatantEntity target;
                
            switch (targetingType)
            {
                case TargetingType.LOW_HEALTH:
                    target = _combatantRepository.Get(combatantStore.LowestHealthCombatant.CombatantID);
                    break;
                case TargetingType.HIGH_ATTACK:
                    target = _combatantRepository.Get(combatantStore.HighestAttackCombatant.CombatantID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackingEntity));
            }

            _objectNullAssertion.AssertNotNull(target, nameof(target));

            return target;
        }

        private TargetingType GetTargetingType(CombatantEntity attackingEntity, SkillType skillType)
        {
            ComponentStore<SkillComponent> skillComponentStore = attackingEntity.GetComponent<ComponentStore<SkillComponent>>();
            _foundAssertion.AssertFound(skillType, skillComponentStore.ContainsComponent(component => component.SkillType == skillType));
            
            foreach (SkillComponent skillComponent in skillComponentStore.GetAllComponents())
            {
                if (skillComponent.SkillType != skillType)
                {
                    continue;
                }

                return skillComponent.TargetingType;
            }

            // TODO: figure out something better
            throw new InvalidOperationException();
        }
    }
}