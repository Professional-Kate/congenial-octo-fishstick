using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Combatant.Mediator
{
    public sealed class CombatantCreationMediator : IBatchMediator<CombatantCreation>
    {
        private readonly IIncrementalRepository<CombatantDefinition> _combatantDefinitionRepository;
        private readonly IDispatchMany<CombatantCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly ICardAsserter _cardAsserter;

        public CombatantCreationMediator(IIncrementalRepository<CombatantDefinition> combatantDefinitionRepository, IDispatchMany<CombatantCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, ICardAsserter cardAsserter)
        {
            _combatantDefinitionRepository = combatantDefinitionRepository;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _cardAsserter = cardAsserter;
        }

        public void HandleMessages(IReadOnlyList<CombatantCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantCreationResponse[] responses = new CombatantCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantCreation combatantCreation = messages[i];
                _cardAsserter.AssertCombatantCards(combatantCreation);

                CombatantDefinition combatantDefinition = new()
                {
                    CombatantID = _combatantDefinitionRepository.GetID(),
                    CombatantType = combatantCreation.CombatantType,
                    StatCard = combatantCreation.StatCard,
                    AgilityCard = combatantCreation.AgilityCard
                };
                
                _combatantDefinitionRepository.Add(combatantDefinition);

                responses[i] = CreateResponse(combatantCreation, combatantDefinition.CombatantID);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static CombatantCreationResponse CreateResponse(CombatantCreation combatantCreation, byte combatantID)
        {
            return new CombatantCreationResponse
            {
                CombatantType = combatantCreation.CombatantType,
                StatCard = combatantCreation.StatCard,
                AgilityCard = combatantCreation.AgilityCard,
                CombatantID = combatantID
            };
        }
    }
}