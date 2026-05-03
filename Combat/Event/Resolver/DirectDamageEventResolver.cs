using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class DirectDamageEventResolver : IEventResolver
    {
        private readonly ITargetFinder _targetFinder;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly IEntityDamageMediator _entityDamageMediator;
        private readonly IBasicAttackScheduler _basicAttackScheduler;
        private readonly ICombatantRepository _combatantRepository;

        public DirectDamageEventResolver(ITargetFinder targetFinder, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IEntityDamageMediator entityDamageMediator, IBasicAttackScheduler basicAttackScheduler, ICombatantRepository combatantRepository)
        {
            _targetFinder = targetFinder;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _entityDamageMediator = entityDamageMediator;
            _basicAttackScheduler = basicAttackScheduler;
            _combatantRepository = combatantRepository;
        }

        public void ResolveEvent(double tick, byte attackerID, AbilityType abilityType)
        {
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackerID);
            if (attackingCombatant.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this Event can resolve
                return;
            }           
            
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(attackerID, abilityType);
            CombatantEntity targetCombatant = _targetFinder.FindBestTarget(attackingCombatant, abilityType);
            
            _entityDamageMediator.ApplyDamage(targetCombatant, attackingCombatant, combatantAbilityEntity);
            _basicAttackScheduler.EnqueueAttack(tick, attackerID, abilityType);
        }
    }
}