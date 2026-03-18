using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatService: ICombatService
    {
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatService(ICollectionAssertion collectionAssertion)
        {
            _collectionAssertion = collectionAssertion;
        }

        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck)
        {
            _collectionAssertion.AssertHasElements(basicEncounterDeck.FriendlyCombatantCards);
            _collectionAssertion.AssertHasElements(basicEncounterDeck.EnemyCombatantCards);
            
            // TODO: Add initial combatants into ECS 
            // TODO: queue attacks
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