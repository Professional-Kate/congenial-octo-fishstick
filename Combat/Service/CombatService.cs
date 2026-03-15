using IdelPog.Combat.Contracts.Card;
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

            return new EncounterResponse
            {
                BasicEncounterDeck = basicEncounterDeck,
                FriendlyWin = false 
            };
        }

        private StatCard CalculateTotalStats(CombatantCard[] combatantCards)
        {
            uint attack = 0;
            uint health = 0;
            
            foreach (CombatantCard combatantCard in combatantCards)
            {
                attack += combatantCard.StatCard.Attack;
                health += combatantCard.StatCard.Health;
            }
            
            return new StatCard { Attack = attack,  Health = health };
        }

        private uint HitsToKill(uint health, uint attack)
        {
            return (health + attack - 1) / attack;
        }
    }
}