using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class BasicEncounterDeckTest : ManagedTestBuffer
    {
        private readonly CombatTools _combatTools = new();
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;
        private ManagedErrorListener<BasicEncounterDeckError> _errorListener;

        private CombatantCreation _humanCreation;
        private CombatantCreation _goblinCreation;
        private CombatantCreation _bearCreation;
        private CombatantCreation _wolfCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCreation = new CombatantCreation
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 14, Attack = 5, Speed = 5 },
                Information = new Information { Name = "John Idle", Description = "He the man" }
            };
            
            _goblinCreation = new CombatantCreation
            {
                CombatantType = CombatantType.GOBLIN, 
                StatCard = new StatCard { Health = 9, Attack = 2, Speed = 11 },
                Information = new Information { Name = "Goblin", Description = "green guy" }
            };

            _bearCreation = new CombatantCreation
            {
                CombatantType = CombatantType.BEAR,
                StatCard = new StatCard { Health = 5, Attack = 10, Speed = 3 },
                Information = new Information { Name = "Bear", Description = "rawr" }
            };
            
            _wolfCreation = new CombatantCreation
            {
                CombatantType = CombatantType.WOLF,
                StatCard = new StatCard { Health = 3, Attack = 7, Speed = 3 },
                Information = new Information { Name = "Wolf", Description = "awoooo" }
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            _errorListener = new ManagedErrorListener<BasicEncounterDeckError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            _combatTools.Reset();
        }

        private void DispatchBasicEncounterDeck(params BasicEncounterDeck[] basicEncounterDecks)
        {
            IBuffer<BasicEncounterDeck> buffer = BufferManager.RequestBuffer<BasicEncounterDeck>(new BufferRequest(basicEncounterDecks.Length));
            buffer.Assign(basicEncounterDecks);
            buffer.MarkReady();
        }

        private BasicEncounterDeck RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchBasicEncounterDeck(basicEncounterDeck);

            return basicEncounterDeck;
        }

        private static void AssertFriendlyVictory(BasicEncounterDeckResponse basicEncounterDeckResponse)
        { 
            Assert.That(basicEncounterDeckResponse.FriendlyVictory, Is.True);
        }
        
        private static void AssertEnemyVictory(BasicEncounterDeckResponse basicEncounterDeckResponse)
        { 
            Assert.That(basicEncounterDeckResponse.FriendlyVictory, Is.False);
        }

        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(BasicEncounterDeck basicEncounterDeck, BasicEncounterDeck expected)
        { 
            Assert.That(basicEncounterDeck, Is.EqualTo(expected));
        }
        
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.BasicEncounterDecks, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params BasicEncounterDeck[] basicEncounterDecks) where TException : Exception
        {
            BasicEncounterDeckError basicEncounterDeckError = _errorListener.Error;
            
            Assert.Multiple(() =>
            {
                Assert.That(basicEncounterDeckError.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(basicEncounterDeckError.BaseError.Exception.GetBaseException(), Is.TypeOf<TException>());
                Assert.That(basicEncounterDecks, Is.EquivalentTo(basicEncounterDeckError.BasicEncounterDecks));
            });
        }

        private void RegisterStateChanges(BasicEncounterDeckResponse basicEncounterDeckResponse)
        { 
            _combatTools.RegisterChanges(basicEncounterDeckResponse.CombatantStateChanges);
        }
        
        private void AssertFirstDead(CombatantCreation creation)
        { 
            Assert.That(creation.Information, Is.EqualTo(_combatTools.FirstDeadCombatant.CombatantCreation.Information));
        }

        private void AssertZeroAttacks(params CombatantCreation[] combatantCards)
        {
            foreach (CombatantCreation combatantCard in combatantCards)
            {
                CombatantTracker tracker = _combatTools.GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.EqualTo(0));
            }
        }

        private void AssertOneOrMoreAttacks(params CombatantCreation[] combatantCards)
        {
            foreach (CombatantCreation combatantCard in combatantCards)
            {
                CombatantTracker tracker = _combatTools.GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.GreaterThanOrEqualTo(1));
            }
        }

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            BasicEncounterDeck returnedDeck = RunCombat([1], [1]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            AssertFriendlyVictory(_responseListener.Responses[0]);
        }
        
        [Test]
        public void Positive_SimulateCombat_EnemyVictory()
        { 
            BasicEncounterDeck returnedDeck = RunCombat([1], [1, 1, 1]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            AssertEnemyVictory(_responseListener.Responses[0]);
        }

        [Test]
        public void Positive_SimulateCombat_HighAttack_TargetsHighAttack()
        {
            AbilityCard highAttackCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK } };
            BasicEncounterDeck returnedDeck = RunCombat([1], [1, 1]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            
            AssertFriendlyVictory(_responseListener.Responses[0]);
            RegisterStateChanges(_responseListener.Responses[0]);
            AssertFirstDead(_bearCreation);
            AssertZeroAttacks(_bearCreation);
            AssertOneOrMoreAttacks(_humanCreation, _goblinCreation);
        }
        
        [Test]
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            AbilityCard lowHealthCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.LOW_HEALTH } };
            BasicEncounterDeck returnedDeck = RunCombat([1], [1, 1]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            
            AssertFriendlyVictory(_responseListener.Responses[0]);
            RegisterStateChanges(_responseListener.Responses[0]);
            AssertFirstDead(_wolfCreation);
            AssertZeroAttacks(_wolfCreation);
            AssertOneOrMoreAttacks(_humanCreation, _bearCreation);
        }
        
        // Exception Tests
        [Test]
        public void Negative_EmptyFriendlyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyFriendlyCombatants = new() { FriendlyCombatantIDs = [], EnemyCombatantIDs = [1] };

            DispatchBasicEncounterDeck(emptyFriendlyCombatants);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyFriendlyCombatants);
        }

        [Test]
        public void Negative_EmptyEnemyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyEnemyCombatants = new() { FriendlyCombatantIDs = [1], EnemyCombatantIDs = [] };
            
            DispatchBasicEncounterDeck(emptyEnemyCombatants);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyEnemyCombatants);
        }

        [Test]
        public void Negative_ZeroSpeed_DispatchesError()
        {
            CombatantCreation zeroSpeed = new()
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 14, Attack = 5, Speed = 0 },
                Information = new Information { Name = "Captain Slow", Description = "The slowest man... In the world" }
            };
            
            BasicEncounterDeck deck = new() { FriendlyCombatantIDs = [1], EnemyCombatantIDs = [1] };
            DispatchBasicEncounterDeck(deck);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(deck);
        }
        
        [Test]
        public void Negative_ZeroHealth_DispatchesError()
        {
            CombatantCreation zeroHealth = new()
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 0, Attack = 5, Speed = 200 },
                Information = new Information { Name = "corpse", Description = "He kinda dead already" }
            };
            
            BasicEncounterDeck deck = new() { FriendlyCombatantIDs = [1], EnemyCombatantIDs = [1] };
            DispatchBasicEncounterDeck(deck);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(deck);
        }
    }
}