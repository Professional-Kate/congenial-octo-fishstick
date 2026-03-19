using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatService : ICombatService
    {
        private readonly ICombatantFactory _combatantFactory;
        private readonly IAttackScheduler _attackScheduler;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatService(ICollectionAssertion collectionAssertion, ICombatantFactory combatantFactory, IAttackScheduler attackScheduler)
        {
            _collectionAssertion = collectionAssertion;
            _combatantFactory = combatantFactory;
            _attackScheduler = attackScheduler;
        }

        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantCards);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantCards);
            
            _combatantFactory.SpawnCombatants(basicEncounterDeck.FriendlyCombatantCards);
            _combatantFactory.SpawnCombatants(basicEncounterDeck.EnemyCombatantCards);
            
            _attackScheduler.EnqueueInitial(0);
            // TODO: run next attack until all dead
            // TODO: fill out EncounterResponse

            return new EncounterResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                FriendlyWin = false 
            };
        }

        private uint HitsToKill(uint health, uint attack)
        {
            return (health + attack - 1) / attack;
        }
    }
}