using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatTest : ManagedTestBuffer
    {
        private readonly CombatTools _combatTools = new();
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;
        private ManagedErrorListener<BasicEncounterDeckError> _errorListener;

        private CombatantCard _humanCard;
        private CombatantCard _goblinCard;
        private CombatantCard _bearCard;
        private CombatantCard _wolfCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCard = new CombatantCard
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 14, Attack = 5, Speed = 5 },
                Information = new Information { Name = "John Idle", Description = "He the man" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }}]
            };
            
            _goblinCard = new CombatantCard
            {
                CombatantType = CombatantType.GOBLIN, 
                StatCard = new StatCard { Health = 9, Attack = 2, Speed = 11 },
                Information = new Information { Name = "Goblin", Description = "green guy" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.LOW_HEALTH }}]
            };

            _bearCard = new CombatantCard
            {
                CombatantType = CombatantType.BEAR,
                StatCard = new StatCard { Health = 5, Attack = 10, Speed = 3 },
                Information = new Information { Name = "Bear", Description = "rawr" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }}]
            };
            
            _wolfCard = new CombatantCard
            {
                CombatantType = CombatantType.WOLF,
                StatCard = new StatCard { Health = 3, Attack = 7, Speed = 3 },
                Information = new Information { Name = "Wolf", Description = "awoooo" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }}]
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

        private BasicEncounterDeck RunCombat(CombatantCard[] friendlyCombatants, CombatantCard[] enemyCombatants)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantCards = friendlyCombatants,
                EnemyCombatantCards = enemyCombatants
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
        
        private void AssertFirstDead(CombatantCard card)
        { 
            Assert.That(card.Information, Is.EqualTo(_combatTools.FirstDeadCombatant.CombatantCard.Information));
        }

        private void AssertZeroAttacks(params CombatantCard[] combatantCards)
        {
            foreach (CombatantCard combatantCard in combatantCards)
            {
                CombatantTracker tracker = _combatTools.GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.EqualTo(0));
            }
        }

        private void AssertOneOrMoreAttacks(params CombatantCard[] combatantCards)
        {
            foreach (CombatantCard combatantCard in combatantCards)
            {
                CombatantTracker tracker = _combatTools.GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.GreaterThanOrEqualTo(1));
            }
        }

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            BasicEncounterDeck returnedDeck = RunCombat([_humanCard], [_goblinCard]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            AssertFriendlyVictory(_responseListener.Responses[0]);
        }
        
        [Test]
        public void Positive_SimulateCombat_EnemyVictory()
        { 
            BasicEncounterDeck returnedDeck = RunCombat([_humanCard], [_goblinCard, _bearCard, _wolfCard]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            AssertEnemyVictory(_responseListener.Responses[0]);
        }

        [Test]
        public void Positive_SimulateCombat_HighAttack_TargetsHighAttack()
        {
            SkillCard highAttackCard = new() { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK } };
            BasicEncounterDeck returnedDeck = RunCombat([_humanCard with { SkillCards = [highAttackCard]}], [_goblinCard, _bearCard]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            
            AssertFriendlyVictory(_responseListener.Responses[0]);
            RegisterStateChanges(_responseListener.Responses[0]);
            AssertFirstDead(_bearCard);
            AssertZeroAttacks(_bearCard);
            AssertOneOrMoreAttacks(_humanCard, _goblinCard);
        }
        
        [Test]
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            SkillCard lowHealthCard = new() { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.LOW_HEALTH } };
            BasicEncounterDeck returnedDeck = RunCombat([_humanCard with { SkillCards = [lowHealthCard]}], [_wolfCard, _bearCard]);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0].BasicEncounterDeck, returnedDeck);
            
            AssertFriendlyVictory(_responseListener.Responses[0]);
            RegisterStateChanges(_responseListener.Responses[0]);
            AssertFirstDead(_wolfCard);
            AssertZeroAttacks(_wolfCard);
            AssertOneOrMoreAttacks(_humanCard, _bearCard);
        }
        
        // Exception Tests
        [Test]
        public void Negative_EmptyFriendlyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyFriendlyCombatants = new() { FriendlyCombatantCards = [], EnemyCombatantCards = [_wolfCard] };

            DispatchBasicEncounterDeck(emptyFriendlyCombatants);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyFriendlyCombatants);
        }

        [Test]
        public void Negative_EmptyEnemyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyEnemyCombatants = new() { FriendlyCombatantCards = [_wolfCard], EnemyCombatantCards = [] };
            
            DispatchBasicEncounterDeck(emptyEnemyCombatants);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyEnemyCombatants);
        }

        [Test]
        public void Negative_ZeroSpeed_DispatchesError()
        {
            CombatantCard zeroSpeed = new()
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 14, Attack = 5, Speed = 0 },
                Information = new Information { Name = "Captain Slow", Description = "The slowest man... In the world" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }}]
            };
            
            BasicEncounterDeck deck = new() { FriendlyCombatantCards = [zeroSpeed], EnemyCombatantCards = [_wolfCard] };
            DispatchBasicEncounterDeck(deck);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(deck);
        }
        
        [Test]
        public void Negative_ZeroHealth_DispatchesError()
        {
            CombatantCard zeroHealth = new()
            {
                CombatantType = CombatantType.HUMAN, 
                StatCard = new StatCard { Health = 0, Attack = 5, Speed = 200 },
                Information = new Information { Name = "corpse", Description = "He kinda dead already" },
                SkillCards = [new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }}]
            };
            
            BasicEncounterDeck deck = new() { FriendlyCombatantCards = [zeroHealth], EnemyCombatantCards = [_wolfCard] };
            DispatchBasicEncounterDeck(deck);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(deck);
        }
    }
}