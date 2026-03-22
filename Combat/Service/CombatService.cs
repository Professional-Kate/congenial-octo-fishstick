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
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICombatQueue _combatQueue;
        private readonly IAssetRepository<EventType, IEventResolver> _resolverRepository;
        private readonly ICombatStateService _combatStateService;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatService(ICombatantFactory combatantFactory, ICombatantStoreService combatantStoreService, IAttackScheduler attackScheduler, ICombatQueue combatQueue, IAssetRepository<EventType, IEventResolver> resolverRepository, ICombatStateService combatStateService, ICollectionAssertion collectionAssertion)
        {
            _combatantFactory = combatantFactory;
            _combatantStoreService = combatantStoreService;
            _attackScheduler = attackScheduler;
            _combatQueue = combatQueue;
            _resolverRepository = resolverRepository;
            _combatStateService = combatStateService;
            _collectionAssertion = collectionAssertion;
        }

        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck)
        {
            RegisterCombatants(basicEncounterDeck);

            while (_combatStateService.IsCombatOver == false)
            { 
                ICombatEvent combatEvent = _combatQueue.Dequeue();
                double currentTick = combatEvent.Tick;

                IEventResolver resolver = _resolverRepository.Get(combatEvent.EventType);
                resolver.ResolveEvent(currentTick, combatEvent.AttackerID);
                
                _combatStateService.Evaluate();
            } 

            return new EncounterResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                FriendlyWin = _combatStateService.FriendlyVictory 
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