using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatService : ICombatService
    {
        private readonly ICombatantFactory _combatantFactory;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICombatQueue _combatQueue;
        private readonly IAssetRepository<EventType, IEventResolver> _resolverRepository;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatService(ICombatantFactory combatantFactory, IAttackScheduler attackScheduler, ICombatQueue combatQueue, IAssetRepository<EventType, IEventResolver> resolverRepository, ICollectionAssertion collectionAssertion, ICombatantStoreService combatantStoreService)
        {
            _combatantFactory = combatantFactory;
            _attackScheduler = attackScheduler;
            _combatQueue = combatQueue;
            _resolverRepository = resolverRepository;
            _collectionAssertion = collectionAssertion;
            _combatantStoreService = combatantStoreService;
        }

        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck)
        {
            RegisterCombatants(basicEncounterDeck);

            while (true)
            { 
                ICombatEvent combatEvent = _combatQueue.Dequeue();
                double currentTick = combatEvent.Tick;

                if (_resolverRepository.Contains(combatEvent.EventType))
                {
                    IEventResolver resolver = _resolverRepository.Get(combatEvent.EventType);
                    resolver.ResolveEvent(currentTick, combatEvent.AttackerID);
                }
                
                // TODO: utility class that decides when combat is over, loop on that.
                // TODO: That class will be reported changes by each Resolver that affects an Entity.
                
                break;
            }

            return new EncounterResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                FriendlyWin = false 
            };
        }

        private void RegisterCombatants(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantCards);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantCards);
            
            _combatantFactory.SpawnCombatants(basicEncounterDeck.FriendlyCombatantCards);
            _combatantFactory.SpawnCombatants(basicEncounterDeck.EnemyCombatantCards);
            
            _combatantStoreService.RegisterInitial();
            _attackScheduler.EnqueueInitial(0);
        }
    }
}