using System.Collections.Immutable;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Combatant.Mediator
{
    public sealed class AbilityEquipMediator : IBatchMediator<AbilityEquip>
    {
        private readonly IAbilitySlotCalculator _abilitySlotCalculator;
        private readonly IPrioritySorter _prioritySorter;
        private readonly IIncrementalRepository<AbilityDefinition> _abilityRepository;
        private readonly IDictionary<byte, EquippedAbilityDefinition> _equippedAbilityRepository;
        private readonly IDispatchMany<AbilityEquipResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IAbilityAssertion _abilityAssertion;
        private readonly IPriorityAssertion _priorityAssertion;

        public AbilityEquipMediator(IAbilitySlotCalculator abilitySlotCalculator, IPrioritySorter prioritySorter,
            IIncrementalRepository<AbilityDefinition> abilityRepository, IDictionary<byte, EquippedAbilityDefinition> equippedAbilityRepository,
            IDispatchMany<AbilityEquipResponse> responseDispatcher, ICollectionAssertion collectionAssertion,
            IAbilityAssertion abilityAssertion, IPriorityAssertion priorityAssertion)
        {
            _abilitySlotCalculator = abilitySlotCalculator;
            _equippedAbilityRepository = equippedAbilityRepository;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _abilityAssertion = abilityAssertion;
            _abilityRepository = abilityRepository;
            _prioritySorter = prioritySorter;
            _priorityAssertion = priorityAssertion;
        }

        public void HandleMessages(IReadOnlyList<AbilityEquip> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            AbilityEquipResponse[] responses = new AbilityEquipResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                AbilityEquip abilityEquip = messages[i];
                _collectionAssertion.AssertHasElements(abilityEquip.EquippedAbilities);
                _abilityAssertion.AssertAbilityCount(_abilitySlotCalculator.GetAbilitySlots(abilityEquip.EquippedAbilities));
                
                EquippedAbilityDefinition equippedAbilityDefinition = new()
                {
                    CombatantID = abilityEquip.CombatantID,
                    EquippedAbilities = SortEquippedAbilities(abilityEquip.EquippedAbilities)
                };
                _equippedAbilityRepository.Add(equippedAbilityDefinition.CombatantID, equippedAbilityDefinition);
                
                responses[i] = new AbilityEquipResponse { CombatantID = abilityEquip.CombatantID };
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private ImmutableArray<EquippedAbility> SortEquippedAbilities(EquippedAbility[] equippedAbilityStages)
        {
            EquippedAbility[] sortedAbilityStages = new EquippedAbility[equippedAbilityStages.Length];
            for (int index = 0; index < equippedAbilityStages.Length; index++)
            {
                EquippedAbility equippedAbility = equippedAbilityStages[index];
                ImmutableArray<StrategyCard> sortedStrategyCards = _prioritySorter.Sort(equippedAbility.StrategyCards, strategyCard => strategyCard.Priority);

                AbilityDefinition abilityDefinition = _abilityRepository.Get(equippedAbility.AbilityID);

                _priorityAssertion.AssertPriority(abilityDefinition.AbilityStages, sortedStrategyCards);
                sortedAbilityStages[index] = equippedAbility with { StrategyCards = [..sortedStrategyCards] };
            }

            return [..sortedAbilityStages];
        }
    }
}