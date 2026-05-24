using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class CombatantCreationMediator : IBatchMediator<CombatantCreation>
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantEntityFactory _combatantEntityFactory;
        private readonly IDispatchMany<CombatantCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly ICardAsserter _cardAsserter;

        public CombatantCreationMediator(ICombatantRepository combatantRepository, ICombatantEntityFactory combatantEntityFactory, IDispatchMany<CombatantCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, ICardAsserter cardAsserter)
        {
            _combatantRepository = combatantRepository;
            _combatantEntityFactory = combatantEntityFactory;
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

                CombatantEntity newEntity = _combatantEntityFactory.CreateEntity(combatantCreation, _combatantRepository.NextCombatantID);
                _combatantRepository.Add(newEntity);

                responses[i] = CreateResponse(combatantCreation, newEntity.CombatantID);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static CombatantCreationResponse CreateResponse(CombatantCreation combatantCreation, byte combatantID)
        {
            return new CombatantCreationResponse
            {
                CombatantType = combatantCreation.CombatantType,
                Information = combatantCreation.Information,
                StatCard = combatantCreation.StatCard,
                AgilityCard = combatantCreation.AgilityCard,
                CombatantID = combatantID
            };
        }
    }
}