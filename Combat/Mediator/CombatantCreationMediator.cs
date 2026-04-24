using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
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
        private readonly IStatCardAsserter _statCardAsserter;

        public CombatantCreationMediator(ICombatantRepository combatantRepository, ICombatantEntityFactory combatantEntityFactory, IDispatchMany<CombatantCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IStatCardAsserter statCardAsserter)
        {
            _combatantRepository = combatantRepository;
            _combatantEntityFactory = combatantEntityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _statCardAsserter = statCardAsserter;
        }

        public void HandleMessages(IReadOnlyList<CombatantCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantCreationResponse[] responses = new CombatantCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantCreation combatantCreation = messages[i];
                _statCardAsserter.AssertStatCard(combatantCreation.StatCard);
                
                _combatantRepository.Add(_combatantEntityFactory.CreateEntity(combatantCreation));

                responses[i] = CreateResponse(combatantCreation);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static CombatantCreationResponse CreateResponse(CombatantCreation combatantCreation)
        {
            return new CombatantCreationResponse
            {
                CombatantType = combatantCreation.CombatantType,
                Information = combatantCreation.Information,
                StatCard = combatantCreation.StatCard
            };
        }
    }
}