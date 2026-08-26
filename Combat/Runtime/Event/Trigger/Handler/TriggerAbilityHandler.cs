using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.Event.Trigger.Handler
{
    public abstract class TriggerAbilityHandler<T> : ITriggerAbilityHandler<T> where T : struct
    {
        private readonly ITriggerReader _triggerReader;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly ICombatantRepository _combatantRepository;

        protected TriggerAbilityHandler(ITriggerReader triggerReader, IAbilityEventScheduler abilityEventScheduler, ICombatantRepository combatantRepository)
        {
            _triggerReader = triggerReader;
            _abilityEventScheduler = abilityEventScheduler;
            _combatantRepository = combatantRepository;
        }

        protected abstract TriggerEventType TriggerEventType { get; }
        
        public void Handle(double tick, T triggerData)
        {
            IEnumerable<AbilityTrigger> abilityTriggers = Filter(_triggerReader.GetAbilities(TriggerEventType), triggerData, tick);
            
            foreach (AbilityTrigger abilityTrigger in abilityTriggers)
            {
                _abilityEventScheduler.ScheduleEvent(abilityTrigger.Tick, abilityTrigger.AbilityID, abilityStageIndex: 0, initiatingCombatantID: abilityTrigger.CombatantID);
            }
        }
        
        protected abstract IEnumerable<AbilityTrigger> Filter(ImmutableArray<CombatantAbilityEntity> combatantAbilityEntities, T triggerData, double tick);
        
        protected bool IsEligible(CombatantAbilityEntity combatantAbilityEntity, TargetingType combatantTargetingType, byte combatantID, TriggerComponent triggerComponent, double tick)
        {
            ReadyTickComponent readyTickComponent = combatantAbilityEntity.GetComponent<ReadyTickComponent>();
            if (readyTickComponent.ReadyTick > tick)
            {
                return false;
            }

            CombatantEntity combatantEntity = GetCombatantEntity(combatantAbilityEntity.CombatantID);
            LifeStatusComponent lifeStatusComponent = combatantEntity.GetComponent<LifeStatusComponent>();
            if (lifeStatusComponent.IsAlive == false)
            {
                return false;
            }

            TargetingTypeComponent targetingTypeComponent = combatantEntity.GetComponent<TargetingTypeComponent>();
            bool isFriendly = targetingTypeComponent.TargetingType == combatantTargetingType;

            if (DoesTargetingTypeMatch(triggerComponent.TargetingType, isFriendly, combatantAbilityEntity.CombatantID, combatantID) == false)
            {
                return false;
            }

            return true;
        }

        protected bool IsValueInRange(uint minValue, uint maxValue, uint actualValue) => actualValue >= minValue && actualValue <= maxValue;
        
        protected CombatantEntity GetCombatantEntity(byte combatantID) => _combatantRepository.Get(combatantID);

        private static bool DoesTargetingTypeMatch(TargetingType triggerTargetingType, bool isFriendly, byte abilityCombatantID, byte combatantID)
        {
            return triggerTargetingType switch
            {
                TargetingType.SELF => abilityCombatantID == combatantID,
                TargetingType.FRIENDLY => isFriendly,
                TargetingType.ENEMY => isFriendly == false,
                _ => throw new ArgumentOutOfRangeException(nameof(triggerTargetingType), triggerTargetingType, null)
            };
        }
    }
}