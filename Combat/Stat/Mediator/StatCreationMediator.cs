using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Stat.Mediator
{
    public sealed class StatCreationMediator : IBatchMediator<StatCreation>
    {
        private readonly IIncrementalRepository<StatCreation> _statCreationRepository;
        private readonly IDispatchMany<StatCreationResponse> _statCreationResponseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public StatCreationMediator(IIncrementalRepository<StatCreation> statCreationRepository, IDispatchMany<StatCreationResponse> statCreationResponseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _statCreationRepository = statCreationRepository;
            _statCreationResponseDispatcher = statCreationResponseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<StatCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            StatCreationResponse[] responses = new StatCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                StatCreation statCreation = messages[i];
                
                byte id = _statCreationRepository.Add(statCreation);
                responses[i] = new StatCreationResponse { StatID = id, StatCreation = statCreation };
            }
            
            _statCreationResponseDispatcher.Dispatch(responses);
        }
    }
}