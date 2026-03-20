using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatService : ICombatService
    {
        private readonly ICombatantFactory _combatantFactory;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICombatQueue _combatQueue;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatService(ICollectionAssertion collectionAssertion, ICombatantFactory combatantFactory, IAttackScheduler attackScheduler, ICombatQueue combatQueue)
        {
            _collectionAssertion = collectionAssertion;
            _combatantFactory = combatantFactory;
            _attackScheduler = attackScheduler;
            _combatQueue = combatQueue;
        }

        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantCards);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantCards);
            
            _combatantFactory.SpawnCombatants(basicEncounterDeck.FriendlyCombatantCards);
            _combatantFactory.SpawnCombatants(basicEncounterDeck.EnemyCombatantCards);
            
            _attackScheduler.EnqueueInitial(0);

            while (true)
            { 
                ICombatEvent combatEvent = _combatQueue.Dequeue();
                double currentTick = combatEvent.Tick;

                break;
            }

            return new EncounterResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                FriendlyWin = false 
            };
        }
    }
}