using IdelPog.Combat;
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
        
        private readonly AbilityCreation _basicAttackCreation = StaticCombatCommands.SlashAttackCreation; 
        private readonly CombatantAbilityEquip _equipBasicAttack = StaticCombatCommands.EquipSlashAttack(0);
        
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(basicEncounterDeckError.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(basicEncounterDeckError.BaseError.Exception.GetBaseException(), Is.TypeOf<TException>());
                Assert.That(basicEncounterDecks, Is.EquivalentTo(basicEncounterDeckError.BasicEncounterDecks));
            }
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
            
            _combatTools.AssertOneOrMoreAttacks(_wolfCreation, _bearCreation, _humanCreation, _goblinCreation);
        }

        [Test]
        public void Positive_SimulateCombat_TargetsHighSpeed()
        {
            CombatantAbilityCard highAttackCard = new() { AbilityType = AbilityType.SLASH, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.SPEED }};
            
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack with { AbilityCards = [highAttackCard] }, _equipBasicAttack with { CombatantID = 1 });
            
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
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            CombatantAbilityCard lowHealthCard = new() { AbilityType = AbilityType.SLASH, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH } };
            
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
        public void Positive_SimulateCombat_CombatantsAttackInOrder_OfInitiative()
        {
            StatCard sharedStatCard = new() { Health = 100 };
            AgilityCard sameSpeedCard = new() { Speed = 10, Initiative = 0 };
            
            CombatantCreation humanCreation = _humanCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 1 }};
            CombatantCreation bearCreation = _bearCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 2 }};
            CombatantCreation goblinCreation = _goblinCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 3 }};
            CombatantCreation wolfCreation = _wolfCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 4 }};

            DispatchMessage(humanCreation, bearCreation, goblinCreation, wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 }, _equipBasicAttack with { CombatantID = 2 }, _equipBasicAttack with { CombatantID = 3 });
            
            RunCombat([1], [0, 3, 2], _combatantCreationResponseListener.Responses);
            
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
            AbilityValidator.AssertAttackers(3, 2, 1, 0);
            AbilityValidator.Reset();
        }

        [Test]
        public void Positive_SimulateCombat_CombatClearsDown_BetweenCommands()
        {
            DispatchMessage(_humanCreation, _goblinCreation, _bearCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(_equipBasicAttack, StaticCombatCommands.EquipSlashAttack(1), StaticCombatCommands.EquipStabAttack(2), StaticCombatCommands.EquipStabAttack(3));
            
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = [1, 3],
                EnemyCombatantIDs = [0, 2]
            };
            
            DispatchMessage(basicEncounterDeck, basicEncounterDeck, basicEncounterDeck, basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(4);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_responseListener.Responses[1].CombatantStateChanges, Is.EqualTo(_responseListener.Responses[0].CombatantStateChanges));
                Assert.That(_responseListener.Responses[2].CombatantStateChanges, Is.EqualTo(_responseListener.Responses[0].CombatantStateChanges));
                Assert.That(_responseListener.Responses[3].CombatantStateChanges, Is.EqualTo(_responseListener.Responses[0].CombatantStateChanges));
            }
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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(maxIterationsException.MaxIterations, Is.EqualTo(maxIterations));
                Assert.That(maxIterationsException.BasicEncounterDeck, Is.EqualTo(basicEncounterDeck));
            }
        }
    }
}