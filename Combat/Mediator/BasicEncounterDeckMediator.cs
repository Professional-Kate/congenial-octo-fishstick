using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class BasicEncounterDeckMediator : IBatchMediator<BasicEncounterDeck>
    {
        private const uint MAX_ITERATIONS = 10000;
        
        private readonly ICombatantEntityFactory _combatantEntityFactory;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IBasicAttackScheduler _basicAttackScheduler;
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatQueue _combatQueue;
        private readonly IAssetRepository<EventType, IEventResolver> _resolverRepository;
        private readonly ICombatantLogger _combatantLogger;
        private readonly IDispatchMany<BasicEncounterDeckResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public BasicEncounterDeckMediator(ICombatantEntityFactory combatantEntityFactory, ICombatantStoreService combatantStoreService,
            IBasicAttackScheduler basicAttackScheduler, ICombatStateService combatStateService, ICombatQueue combatQueue,
            IAssetRepository<EventType, IEventResolver> resolverRepository, ICombatantLogger combatantLogger,
            IDispatchMany<BasicEncounterDeckResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _combatantEntityFactory = combatantEntityFactory;
            _combatantStoreService = combatantStoreService;
            _basicAttackScheduler = basicAttackScheduler;
            _combatStateService = combatStateService;
            _combatQueue = combatQueue;
            _resolverRepository = resolverRepository;
            _combatantLogger = combatantLogger;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<BasicEncounterDeck> messages)
        {
            BasicEncounterDeckResponse[] responses = new BasicEncounterDeckResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                BasicEncounterDeck basicEncounterDeck = messages[i];
                RegisterCombatants(basicEncounterDeck);

                uint iterations = 0;
                while (_combatStateService.IsCombatOver == false)
                {
                    if (++iterations > MAX_ITERATIONS)
                    {
                        return;
                    }
                    
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
            
            _combatantEntityFactory.SpawnCombatants(basicEncounterDeck.FriendlyCombatantCards, true);
            _combatantEntityFactory.SpawnCombatants(basicEncounterDeck.EnemyCombatantCards, false);
            
            _combatantStoreService.RegisterInitial();
            _basicAttackScheduler.EnqueueInitial(0);
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