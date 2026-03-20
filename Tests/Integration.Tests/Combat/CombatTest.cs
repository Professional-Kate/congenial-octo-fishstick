using IdelPog.Combat;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatTest
    {
        private ICombatService _combatService;
        private BasicEncounterDeck _basicEncounterDeck;

        private CombatantCard _humanCard;
        private CombatantCard _goblinCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCard = new CombatantCard
            {
                CombatantType = CombatantType.HUMAN, 
                IsFriendly = true, 
                StatCard = new StatCard { Health = 9, Attack = 5, Speed = 5 }
            };
            
            _goblinCard = new CombatantCard
            {
                CombatantType = CombatantType.GOBLIN, 
                IsFriendly = false, 
                StatCard = new StatCard { Health = 5, Attack = 3, Speed = 10 }
            };
            
            _basicEncounterDeck = new BasicEncounterDeck
            {
                FriendlyCombatantCards = [_humanCard],
                EnemyCombatantCards = [_goblinCard]
            };
        }
        
        [SetUp]
        public void Setup()
        { 
            _combatService = CombatBootstrapper.SetupCombat();
        }

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            _combatService.RunEncounter(_basicEncounterDeck);
        }
    }
}