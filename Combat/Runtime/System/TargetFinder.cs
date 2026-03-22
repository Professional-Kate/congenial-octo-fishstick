using IdelPog.Combat.Contracts;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TargetFinder : ITargetFinder
    {
        private readonly ICombatantStoreRead _friendlyCombatantStore;
        private readonly ICombatantStoreRead _enemyCombatantStore;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public TargetFinder(ICombatantStoreRead friendlyCombatantStore, ICombatantStoreRead enemyCombatantStore, ICombatantRepository combatantRepository, IObjectNullAssertion objectNullAssertion)
        {
            _friendlyCombatantStore = friendlyCombatantStore;
            _enemyCombatantStore = enemyCombatantStore;
            _combatantRepository = combatantRepository;
            _objectNullAssertion = objectNullAssertion;
        }

        public CombatantEntity FindBestTarget(CombatantEntity attackingEntity)
        {
            return DetermineTarget(attackingEntity.IsFriendly ? _enemyCombatantStore : _friendlyCombatantStore, attackingEntity);
        }

        private CombatantEntity DetermineTarget(ICombatantStoreRead combatantStore, CombatantEntity attackingEntity)
        {
            TargetingTypeComponent targetingTypeComponent = attackingEntity.GetComponent<TargetingTypeComponent>();
            CombatantEntity target;
                
            switch (targetingTypeComponent.TargetingType)
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
    }
}