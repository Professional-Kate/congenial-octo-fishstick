using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class BasicEncounterDeckMediator : IBatchMediator<BasicEncounterDeck>
    {
        private readonly IFriendlyStatusAssigner _friendlyStatusAssigner;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IInitialAbilityScheduler _initialAbilityScheduler;
        private readonly ICombatQueueRunner _combatQueueRunner;
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatantLogger _combatantLogger;
        private readonly IDispatchMany<BasicEncounterDeckResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public BasicEncounterDeckMediator(IFriendlyStatusAssigner friendlyStatusAssigner, ICombatantStoreService combatantStoreService, IInitialAbilityScheduler initialAbilityScheduler, 
            ICombatQueueRunner combatQueueRunner, ICombatStateService combatStateService, ICombatantLogger combatantLogger,
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _friendlyStatusAssigner = friendlyStatusAssigner;
            _combatantStoreService = combatantStoreService;
            _initialAbilityScheduler = initialAbilityScheduler;
            _combatQueueRunner = combatQueueRunner;
            _combatStateService = combatStateService;
            _combatantLogger = combatantLogger;
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
                RegisterCombatants(basicEncounterDeck);

                _combatQueueRunner.RunDeck(basicEncounterDeck);
                
                responses[i] = ConstructResponse(basicEncounterDeck);
                _combatantLogger.ClearStateChanges();
            }

            _responseDispatcher.Dispatch(responses);
        }
        
        private void RegisterCombatants(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantIDs);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantIDs);
            
            _friendlyStatusAssigner.AssignFriendlyStatus(basicEncounterDeck.FriendlyCombatantIDs, basicEncounterDeck.EnemyCombatantIDs);
            _combatantStoreService.RegisterInitialTargets();
            
            _initialAbilityScheduler.EnqueueInitial(0);
        }

        private BasicEncounterDeckResponse ConstructResponse(BasicEncounterDeck basicEncounterDeck)
        {
            return new BasicEncounterDeckResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                CombatantStateChanges = _combatantLogger.GetStateChanges().ToArray(),
                FriendlyVictory = _combatStateService.FriendlyVictory
            };
        }
    }
}