using IdelPog.Combat.Contracts;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TargetFinder : ITargetFinder
    {
        private readonly ICombatantStore _friendlyCombatantStore;
        private readonly ICombatantStore _enemyCombatantStore;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public TargetFinder(ICombatantStore friendlyCombatantStore, ICombatantStore enemyCombatantStore, ICombatantRepository combatantRepository, IObjectNullAssertion objectNullAssertion)
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

        private CombatantEntity DetermineTarget(ICombatantStore combatantSelector, CombatantEntity attackingEntity)
        {
            TargetingTypeComponent targetingTypeComponent = attackingEntity.GetComponent<TargetingTypeComponent>();
            CombatantEntity target;
                
            switch (targetingTypeComponent.TargetingType)
            {
                case TargetingType.LOW_HEALTH:
                    target = _combatantRepository.Get(combatantSelector.LowestHealthCombatant.CombatantID);
                    break;
                case TargetingType.HIGH_ATTACK:
                    target = _combatantRepository.Get(combatantSelector.HighestAttackCombatant.CombatantID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackingEntity));
            }

            _objectNullAssertion.AssertNotNull(target, nameof(target));

            return target;
        }
    }
}