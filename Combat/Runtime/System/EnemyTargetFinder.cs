using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class EnemyTargetFinder : ITargetFinder
    {
        private readonly ICombatantStoreRead _friendlyCombatantStore;
        private readonly ICombatantStoreRead _enemyCombatantStore;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public EnemyTargetFinder(ICombatantStoreRead friendlyCombatantStore, ICombatantStoreRead enemyCombatantStore, ICombatantRepository combatantRepository, IObjectNullAssertion objectNullAssertion, IFoundAssertion foundAssertion)
        {
            _friendlyCombatantStore = friendlyCombatantStore;
            _enemyCombatantStore = enemyCombatantStore;
            _combatantRepository = combatantRepository;
            _objectNullAssertion = objectNullAssertion;
            _foundAssertion = foundAssertion;
        }

        public CombatantEntity FindBestTarget(CombatantEntity instigatingEntity, SkillType skillType)
        {
            return DetermineTarget(instigatingEntity.IsFriendly ? _enemyCombatantStore : _friendlyCombatantStore, instigatingEntity, skillType);
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
            // foreach (BasicAttackComponent skillComponent in skillComponentStore.GetAllComponents())
            // {
            //     if (skillComponent.SkillType != skillType)
            //     {
            //         continue;
            //     }
            //
            //     return skillComponent.TargetingType;
            // }
            //
            // // TODO: figure out something better
            // throw new InvalidOperationException();

            return TargetingType.HIGH_ATTACK;
        }
    }
}