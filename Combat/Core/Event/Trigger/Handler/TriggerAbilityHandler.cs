using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;

namespace IdelPog.Combat.Core.Event.Trigger.Handler
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
        
        protected abstract IEnumerable<AbilityTrigger> Filter(ImmutableArray<AbilityEntity> combatantAbilityEntities, T triggerData, double tick);
        
        protected bool IsEligible(AbilityEntity abilityEntity, TargetingType combatantTargetingType, byte combatantID, TriggerComponent triggerComponent, double tick)
        {
            ReadyTickComponent readyTickComponent = abilityEntity.GetComponent<ReadyTickComponent>();
            if (readyTickComponent.ReadyTick > tick)
            {
                return false;
            }

            CombatantEntity combatantEntity = GetCombatantEntity(abilityEntity.InstanceID);
            LifeStatusComponent lifeStatusComponent = combatantEntity.GetComponent<LifeStatusComponent>();
            if (lifeStatusComponent.IsAlive == false)
            {
                return false;
            }

            bool isFriendly = combatantEntity.TargetingType == combatantTargetingType;
            if (DoesTargetingTypeMatch(triggerComponent.TargetingType, isFriendly, abilityEntity.InstanceID, combatantID) == false)
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