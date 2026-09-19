using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Arena;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Core.Mediator
{
    public sealed class BasicEncounterDeckMediator : IBatchMediator<BasicEncounterDeck>
    {
        private readonly IIncrementalRepository<CombatantDefinition> _combatantDefinitionRepository;
        private readonly ICombatArena _combatArena;
        private readonly IDispatchMany<BasicEncounterDeckResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public BasicEncounterDeckMediator(IIncrementalRepository<CombatantDefinition> combatantDefinitionRepository, ICombatArena combatArena, IDispatchMany<BasicEncounterDeckResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _combatantDefinitionRepository = combatantDefinitionRepository;
            _combatArena = combatArena;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<BasicEncounterDeck> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            BasicEncounterDeckResponse[] responses = new BasicEncounterDeckResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                BasicEncounterDeck basicEncounterDeck = messages[i];
                _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantIDs);
                _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantIDs);
                
                CombatArenaLog combatArenaLog = _combatArena.RunCombatSimulation(GetCombatantDefinitions(basicEncounterDeck.FriendlyCombatantIDs), GetCombatantDefinitions(basicEncounterDeck.EnemyCombatantIDs));

                responses[i] = new BasicEncounterDeckResponse
                {
                    BasicEncounterDeck = basicEncounterDeck,
                    CombatArenaLog = combatArenaLog
                };
            }

            _responseDispatcher.Dispatch(responses);
        }
        
        private CombatantDefinition[] GetCombatantDefinitions(byte[] combatantIDs)
        {
            CombatantDefinition[] combatantDefinitions = new CombatantDefinition[combatantIDs.Length];
            for (int i = 0; i < combatantIDs.Length; i++)
            {
                byte combatantID = combatantIDs[i];
                combatantDefinitions[i] = _combatantDefinitionRepository.Get(combatantID);
            }
            
            return combatantDefinitions;
        }
    }
}