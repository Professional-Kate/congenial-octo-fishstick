using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class BasicEncounterDeckMediator : IBatchMediator<BasicEncounterDeck>
    {
        private readonly ICombatantFactory _combatantFactory;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICombatQueue _combatQueue;
        private readonly IAssetRepository<EventType, IEventResolver> _resolverRepository;
        private readonly ICombatantLogger _combatantLogger;
        private readonly IDispatchMany<BasicEncounterDeckResponse> _responseDispatcher;
        private readonly ICombatStateService _combatStateService;
        private readonly ICollectionAssertion _collectionAssertion;

        public BasicEncounterDeckMediator(ICombatantFactory combatantFactory, ICombatantStoreService combatantStoreService, IAttackScheduler attackScheduler, ICombatQueue combatQueue, IAssetRepository<EventType, IEventResolver> resolverRepository, ICombatStateService combatStateService, ICollectionAssertion collectionAssertion, IDispatchMany<BasicEncounterDeckResponse> responseDispatcher, ICombatantLogger combatantLogger)
        {
            _combatantFactory = combatantFactory;
            _combatantStoreService = combatantStoreService;
            _attackScheduler = attackScheduler;
            _combatQueue = combatQueue;
            _resolverRepository = resolverRepository;
            _combatStateService = combatStateService;
            _collectionAssertion = collectionAssertion;
            _responseDispatcher = responseDispatcher;
            _combatantLogger = combatantLogger;
        }

        public void HandleMessages(IReadOnlyList<BasicEncounterDeck> messages)
        {
            BasicEncounterDeckResponse[] responses = new BasicEncounterDeckResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                BasicEncounterDeck basicEncounterDeck = messages[i];
                RegisterCombatants(basicEncounterDeck);

                while (_combatStateService.IsCombatOver == false)
                {
                    ICombatEvent combatEvent = _combatQueue.Dequeue();
                    double currentTick = combatEvent.Tick;

                    IEventResolver resolver = _resolverRepository.Get(combatEvent.EventType);
                    resolver.ResolveEvent(currentTick, combatEvent.AttackerID);
                }

                responses[i] = ConstructResponse(basicEncounterDeck);
                _combatantLogger.ClearStateChanges();
            }

            _responseDispatcher.Dispatch(responses);
        }
        
        private void RegisterCombatants(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantCards);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantCards);
            
            _combatantFactory.SpawnCombatants(basicEncounterDeck.FriendlyCombatantCards, true);
            _combatantFactory.SpawnCombatants(basicEncounterDeck.EnemyCombatantCards, false);
            
            _combatantStoreService.RegisterInitial();
            _attackScheduler.EnqueueInitial(0);
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