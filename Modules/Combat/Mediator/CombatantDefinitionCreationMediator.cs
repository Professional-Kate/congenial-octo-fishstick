using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class CombatantDefinitionCreationMediator : IBatchMediator<CombatantDefinitionCreation>
    {
        private readonly IAssetRepository<CombatantType, CombatantDefinition> _combatantRepository;
        private readonly ICombatantDefinitionFactory _combatantDefinitionFactory;
        private readonly IDispatchMany<CombatantDefinitionCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly ICombatantStatsAssertion _combatantStatsAssertion;

        public CombatantDefinitionCreationMediator(IAssetRepository<CombatantType, CombatantDefinition> combatantRepository, ICombatantDefinitionFactory combatantDefinitionFactory, IDispatchMany<CombatantDefinitionCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, ICombatantStatsAssertion combatantStatsAssertion)
        {
            _combatantRepository = combatantRepository;
            _combatantDefinitionFactory = combatantDefinitionFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _combatantStatsAssertion = combatantStatsAssertion;
        }

        public void HandleMessages(IReadOnlyList<CombatantDefinitionCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantDefinitionCreationResponse[] responses = new CombatantDefinitionCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantDefinitionCreation creation = messages[i];
                
                _uniqueAssertion.AssertUnique(creation.CombatantType, _combatantRepository.Contains(creation.CombatantType));
                _combatantStatsAssertion.AssertStats(creation.CombatantStats);
                
                _combatantRepository.Add(creation.CombatantType, _combatantDefinitionFactory.Create(creation));
                
                responses[i] = new CombatantDefinitionCreationResponse { CombatantType = creation.CombatantType, CombatantStats = creation.CombatantStats, Information = creation.Information };
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}