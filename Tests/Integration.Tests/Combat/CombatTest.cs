using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Core.Messaging.Buffer;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatTest : ManagedTestBuffer
    {
        private BasicEncounterDeck _basicEncounterDeck;
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;
        private ManagedErrorListener<BasicEncounterDeckError> _errorListener;

        private CombatantCard _humanCard;
        private CombatantCard _goblinCard;
        private CombatantCard _bearCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCard = new CombatantCard
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 90, Attack = 5, Speed = 5 },
                TargetingType = TargetingType.HIGH_ATTACK
            };
            
            _goblinCard = new CombatantCard
            {
                CombatantType = CombatantType.GOBLIN, 
                StatCard = new StatCard { Health = 9, Attack = 2, Speed = 11 },
                TargetingType = TargetingType.LOW_HEALTH
            };

            _bearCard = new CombatantCard
            {
                CombatantType = CombatantType.BEAR,
                StatCard = new StatCard { Health = 5, Attack = 10, Speed = 3 },
                TargetingType = TargetingType.HIGH_ATTACK
            };
            
            _basicEncounterDeck = new BasicEncounterDeck
            {
                FriendlyCombatantCards = [_humanCard],
                EnemyCombatantCards = [_goblinCard, _bearCard]
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            _errorListener = new ManagedErrorListener<BasicEncounterDeckError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }

        private void DispatchBasicEncounterDeck(params BasicEncounterDeck[] basicEncounterDecks)
        {
            IBuffer<BasicEncounterDeck> buffer = BufferManager.RequestBuffer<BasicEncounterDeck>(new BufferRequest(basicEncounterDecks.Length));
            buffer.Assign(basicEncounterDecks);
            buffer.MarkReady();
        }

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            DispatchBasicEncounterDeck(_basicEncounterDeck);
        }
    }
}