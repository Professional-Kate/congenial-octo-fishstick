using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Stat.Mediator
{
    public sealed class StatConversionCreationMediator : IBatchMediator<StatConversionCreation>
    {
        private readonly IIncrementalRepository<StatConversionCreation> _statConversionRepository;
        private readonly IDispatchMany<StatConversionCreationResponse> _statConversionResponseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly INumberAssertion _numberAssertion;

        public StatConversionCreationMediator(IIncrementalRepository<StatConversionCreation> statConversionRepository, IDispatchMany<StatConversionCreationResponse> statConversionResponseDispatcher, ICollectionAssertion collectionAssertion, INumberAssertion numberAssertion)
        {
            _statConversionRepository = statConversionRepository;
            _statConversionResponseDispatcher = statConversionResponseDispatcher;
            _collectionAssertion = collectionAssertion;
            _numberAssertion = numberAssertion;
        }

        public void HandleMessages(IReadOnlyList<StatConversionCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            StatConversionCreationResponse[] responses = new StatConversionCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                StatConversionCreation statConversionCreation = messages[i];
                _numberAssertion.AssertNumberNotZeroOrNegative(statConversionCreation.SourceStatInterval, nameof(statConversionCreation.SourceStatInterval));
                _numberAssertion.AssertNumberNotZero(statConversionCreation.TargetStatModifier, nameof(statConversionCreation.TargetStatModifier));
                
                byte id = _statConversionRepository.Add(statConversionCreation);
                responses[i] = new StatConversionCreationResponse { StatConversionID = id, StatConversionCreation = statConversionCreation };
            }
            
            _statConversionResponseDispatcher.Dispatch(responses);
        }
    }
}