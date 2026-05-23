using IdelPog.Combat;
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
        private ManagedResponseListener<CombatantCreationResponse> _combatantCreationResponseListener;

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
            _combatantCreationResponseListener = new ManagedResponseListener<CombatantCreationResponse>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            ManagedSubscribe(_combatantCreationResponseListener);
            _combatTools.Reset();
        }

        private BasicEncounterDeck RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs, CombatantCreationResponse[] responses)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _responseListener.AssertResponseLength(1);
            _combatTools.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges, responses);
            
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
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertVictory(_responseListener.Responses[0], true);
            
            _combatTools.AssertOneOrMoreAttacks(_humanCreation, _goblinCreation);
        }
        
        [Test]
        public void Positive_SimulateCombat_EnemyVictory()
        { 
            DispatchMessage(_humanCreation, _goblinCreation, _wolfCreation, _bearCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 }, _equipBasicAttack with { CombatantID = 2 }, _equipBasicAttack with { CombatantID = 3 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3], _combatantCreationResponseListener.Responses);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertVictory(_responseListener.Responses[0], false);
            
            _combatTools.AssertZeroAttacks(_humanCreation, _goblinCreation);
            _combatTools.AssertOneOrMoreAttacks(_wolfCreation, _bearCreation);
        }

        [Test]
        public void Positive_SimulateCombat_HighAttack_TargetsHighAttack()
        {
            AbilityCard highAttackCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK } };
            
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack with { AbilityCards = [highAttackCard] }, _equipBasicAttack with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3], _combatantCreationResponseListener.Responses);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertFirstDeadCombatant(_combatTools.FirstDeadCombatant.CombatantCreation,_bearCreation);
            
            _combatTools.AssertZeroAttacks(_bearCreation, _wolfCreation);
            _combatTools.AssertOneOrMoreAttacks(_humanCreation, _goblinCreation);

        }
        
        [Test]
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            AbilityCard lowHealthCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.LOW_HEALTH } };
            
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack with { AbilityCards = [lowHealthCard] }, _equipBasicAttack with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3], _combatantCreationResponseListener.Responses);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            CombatTools.AssertFirstDeadCombatant(_combatTools.FirstDeadCombatant.CombatantCreation,_wolfCreation);
            
            _combatTools.AssertZeroAttacks(_bearCreation, _wolfCreation);
            _combatTools.AssertOneOrMoreAttacks(_humanCreation, _goblinCreation);
        }

        [Test]
        public void Positive_SimulateCombat_CombatantsAttackInOrder()
        { 
            CombatantCreation humanCreation = _humanCreation with { StatCard = new StatCard { Attack = 1, Health = 100, Speed = 10 }};
            CombatantCreation bearCreation = _bearCreation with { StatCard = new StatCard { Attack = 1, Health = 100, Speed = 11 }};
            CombatantCreation goblinCreation = _goblinCreation with { StatCard = new StatCard { Attack = 1, Health = 100, Speed = 12 }};
            CombatantCreation wolfCreation =_wolfCreation with { StatCard = new StatCard { Attack = 1, Health = 100, Speed = 13 }};

            DispatchMessage(humanCreation, bearCreation, goblinCreation, wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 }, _equipBasicAttack with { CombatantID = 2 }, _equipBasicAttack with { CombatantID = 3 });
            
            RunCombat([0], [1, 2, 3], _combatantCreationResponseListener.Responses);
            
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
            AbilityValidator.AssertAttackers(3, 2, 1, 0);
            AbilityValidator.Reset();
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
        public void Negative_LowDamage_HighHealth_ReachesMaxIterations_DispatchesError()
        {
            const uint maxIterations = 1;
            const byte maxCombatantAbilities = 3;
            
            RegisterWithOptions(new CombatOptions { MaxIterations = maxIterations, MaxCombatantAbilitySlots = maxCombatantAbilities });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = [0],
                EnemyCombatantIDs = [1]
            };
            
            DispatchMessage(_humanCreation, _goblinCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 });

            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<MaxIterationsException>(basicEncounterDeck);

            MaxIterationsException maxIterationsException = (_errorListener.Error.BaseError.Exception.GetBaseException() as MaxIterationsException)!;
            Assert.Multiple(() =>
            {
                Assert.That(maxIterationsException.MaxIterations, Is.EqualTo(maxIterations));
                Assert.That(maxIterationsException.BasicEncounterDeck, Is.EqualTo(basicEncounterDeck));
            });
        }
    }
}