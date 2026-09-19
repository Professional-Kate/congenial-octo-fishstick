using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Stat.Mediator
{
    public sealed class StatConfigurationMediator : IBatchMediator<StatConfiguration>
    {
        private readonly IStatConfigurationSetter _statConfigurationSetter;
        private readonly IDispatchMany<StatConfigurationResponse> _statConfigurationResponseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public StatConfigurationMediator(IStatConfigurationSetter statConfigurationSetter, IDispatchMany<StatConfigurationResponse> statConfigurationResponseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _statConfigurationSetter = statConfigurationSetter;
            _statConfigurationResponseDispatcher = statConfigurationResponseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<StatConfiguration> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            StatConfigurationResponse[] responses = new StatConfigurationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                StatConfiguration statConfiguration = messages[i];
                
                _statConfigurationSetter.SetStatConfiguration(statConfiguration);
                
                responses[i] = new StatConfigurationResponse { StatConfiguration = statConfiguration, StatBindings = _statConfigurationSetter.GetStatBindings() };
            }
            
            _statConfigurationResponseDispatcher.Dispatch(responses);
        }
    }
}