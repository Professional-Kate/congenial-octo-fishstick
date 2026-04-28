using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
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

        private readonly CombatantCreation _humanCreation = StaticCombatCommands.HumanCreation;
        private readonly CombatantCreation _goblinCreation = StaticCombatCommands.GoblinCreation;
        private readonly CombatantCreation _bearCreation = StaticCombatCommands.BearCreation;
        private readonly CombatantCreation _wolfCreation = StaticCombatCommands.WolfCreation;
        
        private readonly AbilityCreation _basicAttackCreation = StaticCombatCommands.BasicAttackCreation; 
        private readonly CombatantAbilityEquip _equipBasicAttack = StaticCombatCommands.EquipBasicAttack(0);
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            _errorListener = new ManagedErrorListener<BasicEncounterDeckError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            _combatTools.Reset();
        }

        private BasicEncounterDeck RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _responseListener.AssertResponseLength(1);
            _combatTools.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
            
            return basicEncounterDeck;
        }
        private static void AssertResponse(BasicEncounterDeckResponse basicEncounterDeckResponse, BasicEncounterDeck source)
        { 
            Assert.That(basicEncounterDeckResponse.BasicEncounterDeck, Is.EqualTo(source));
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

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            DispatchMessage(_humanCreation, _goblinCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertVictory(_responseListener.Responses[0], true);
        }
        
        [Test]
        public void Positive_SimulateCombat_EnemyVictory()
        { 
            DispatchMessage(_humanCreation, _goblinCreation, _wolfCreation, _bearCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 }, _equipBasicAttack with { CombatantID = 2 }, _equipBasicAttack with { CombatantID = 3 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertVictory(_responseListener.Responses[0], false);
        }

        [Test]
        public void Positive_SimulateCombat_HighAttack_TargetsHighAttack()
        {
            AbilityCard highAttackCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK } };
            
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack with { AbilityCards = [highAttackCard] }, _equipBasicAttack with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            CombatTools.PrintStateChanges(_responseListener.Responses[0].CombatantStateChanges);
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatTools.AssertFirstDeadCombatant(_combatTools.FirstDeadCombatant.CombatantCreation,_bearCreation);
            _combatTools.AssertZeroAttacks(_bearCreation);
        }
        
        [Test]
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            AbilityCard lowHealthCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.LOW_HEALTH } };
            
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack with { AbilityCards = [lowHealthCard] }, _equipBasicAttack with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatTools.AssertFirstDeadCombatant(_combatTools.FirstDeadCombatant.CombatantCreation,_wolfCreation);
            _combatTools.AssertZeroAttacks(_bearCreation, _wolfCreation);
            _combatTools.AssertOneOrMoreAttacks(_humanCreation, _goblinCreation);
        }
        
        // Exception Tests
        [Test]
        public void Negative_EmptyFriendlyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyFriendlyCombatants = new() { FriendlyCombatantIDs = [], EnemyCombatantIDs = [1] };

            DispatchMessage(emptyFriendlyCombatants);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyFriendlyCombatants);
        }

        [Test]
        public void Negative_EmptyEnemyCombatants_DispatchesError()
        {
            BasicEncounterDeck emptyEnemyCombatants = new() { FriendlyCombatantIDs = [1], EnemyCombatantIDs = [] };
            
            DispatchMessage(emptyEnemyCombatants);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyEnemyCombatants);
        }

        [Test]
        // [Ignore("Takes 130ms to run (more time than all of these tests combined), if impatient, uncomment this!!!")]
        public void Negative_LowDamage_HighHealth_ReachesMaxIterations_DispatchesError()
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = [0],
                EnemyCombatantIDs = [1]
            };
            
            StatCard beefyBoiStats = new() { Health = uint.MaxValue, Attack = 1, Speed = 100 };
            DispatchMessage(_humanCreation with { StatCard = beefyBoiStats }, _goblinCreation with { StatCard = beefyBoiStats });
            DispatchMessage(_basicAttackCreation with { Damage = 1 });
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 });

            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<MaxIterationsException>(basicEncounterDeck);

            MaxIterationsException maxIterationsException = (_errorListener.Error.BaseError.Exception.GetBaseException() as MaxIterationsException)!;
            Assert.Multiple(() =>
            {
                Assert.That(maxIterationsException.MaxIterations, Is.EqualTo(10000));
                Assert.That(maxIterationsException.BasicEncounterDeck, Is.EqualTo(basicEncounterDeck));
            });
        }
    }
}