using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Contracts.Response;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Ability.Mediator
{
    public sealed class AbilityCreationMediator : IBatchMediator<AbilityCreation>
    {
        private readonly IIncrementalRepository<AbilityDefinition> _abilityDefinitionRepository;
        private readonly IPrioritySorter _prioritySorter;
        private readonly IDispatchMany<AbilityCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly INumberAssertion _numberAssertion;
        private readonly ITriggerAssertion _triggerAssertion;

        public AbilityCreationMediator(IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository, IPrioritySorter prioritySorter, IDispatchMany<AbilityCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, INumberAssertion numberAssertion, ITriggerAssertion triggerAssertion)
        {
            _abilityDefinitionRepository = abilityDefinitionRepository;
            _prioritySorter = prioritySorter;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _numberAssertion = numberAssertion;
            _triggerAssertion = triggerAssertion;
        }

        public void HandleMessages(IReadOnlyList<AbilityCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            ValidateCreations(messages);
            
            AbilityCreationResponse[] responses = new AbilityCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                AbilityCreation abilityCreation = messages[i];

                AbilityDefinition abilityDefinition = new()
                {
                    AbilityCard = abilityCreation.AbilityCard,
                    TriggerCard = abilityCreation.TriggerCard,
                    AbilityStages = _prioritySorter.Sort(abilityCreation.AbilityStageCards, stageCard => stageCard.Priority)
                };
                
                byte abilityID = _abilityDefinitionRepository.Add(abilityDefinition);
                responses[i] = CreateResponse(abilityCreation, abilityID);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private void ValidateCreations(IReadOnlyList<AbilityCreation> messages)
        {
            // TODO: instead of validating here we should validate once before this class.
            //  could be something to add to the FlowRegister, CommandValidator
            foreach (AbilityCreation abilityCreation in messages)
            {
                _numberAssertion.AssertNumberNotZero(abilityCreation.AbilityCard.Cooldown, abilityCreation.ToString());
                _collectionAssertion.AssertHasElements(abilityCreation.AbilityStageCards);
                _triggerAssertion.AssertTrigger(abilityCreation.TriggerCard);
                
                foreach (AbilityStageCard abilityStageCard in abilityCreation.AbilityStageCards)
                { 
                    _numberAssertion.AssertNumberNotZero(abilityStageCard.MaxTargets, abilityStageCard.ToString());
                }
            }
        }
        
        private static AbilityCreationResponse CreateResponse(AbilityCreation abilityCreation, byte abilityID)
        {
            return new AbilityCreationResponse
            {
                AbilityID = abilityID,
                AbilityCard = abilityCreation.AbilityCard,
                TriggerCard = abilityCreation.TriggerCard
            };
        }
    }
}