using IdelPog.Combat;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Error;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class BasicEncounterDeckTest : ManagedTestBuffer
    {
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;
        private ManagedErrorListener<BasicEncounterDeckError> _errorListener;
        private ManagedResponseListener<CombatantCreationResponse> _combatantCreationResponseListener;
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            _errorListener = new ManagedErrorListener<BasicEncounterDeckError>();
            _combatantCreationResponseListener = new ManagedResponseListener<CombatantCreationResponse>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            ManagedSubscribe(_combatantCreationResponseListener);
        }
        
        [TearDown]
        public void TearDown()
        {
            CombatValidator.Reset();
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
            CombatValidator.RegisterCombatStages(_responseListener.Responses[0].CombatStages);
            
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
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(1));
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], true);
            CombatValidator.AssertNextInitiatingCombatant(1, 0);
            CombatValidator.AssertCombatantDidAttack(0, 1);
        }
        
        [Test]
        public void Positive_SimulateCombat_EnemyVictory()
        { 
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.WolfCreation, StaticCombatCommands.BearCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 1 }, StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 2 }, StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 3 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], false);
            CombatValidator.AssertCombatantDidAttack(0, 1, 2, 3);
        }

        [Test]
        public void Positive_SimulateCombat_TargetsHighSpeed()
        {
            EquippedAbility highAttack = new() { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.SPEED, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0) with { EquippedAbilities = [highAttack] }, StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatValidator.AssertFirstDeadCombatant(3);
            CombatValidator.AssertNextInitiatingCombatant(1, 0);
            CombatValidator.AssertCombatantDidAttack(0, 1);
            CombatValidator.AssertCombatantDidNotAttack(2, 3);

        }
        
        [Test]
        public void Positive_SimulateCombat_LowHealth_TargetsLowHealth()
        {
            EquippedAbility lowHealth = new() { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0) with { EquippedAbilities = [lowHealth] }, StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 1 });
            
            BasicEncounterDeck returnedDeck = RunCombat([0], [1, 2, 3]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatValidator.AssertFirstDeadCombatant(1);
            CombatValidator.AssertNextInitiatingCombatant(1, 0);
            CombatValidator.AssertCombatantDidAttack(0, 1);
            CombatValidator.AssertCombatantDidNotAttack(2, 3);
        }

        [Test]
        public void Positive_SimulateCombat_CombatantsAttackInOrder_OfInitiative()
        {
            StatCard sharedStatCard = new() { Health = 100 };
            AgilityCard sameSpeedCard = new() { Speed = 10, Initiative = 0 };
            
            CombatantCreation humanCreation = StaticCombatCommands.HumanCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 1 }};
            CombatantCreation bearCreation = StaticCombatCommands.BearCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 2 }};
            CombatantCreation goblinCreation = StaticCombatCommands.GoblinCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 3 }};
            CombatantCreation wolfCreation = StaticCombatCommands.WolfCreation with { StatCard = sharedStatCard, AgilityCard = sameSpeedCard with { Initiative = 4 }};

            DispatchMessage(humanCreation, bearCreation, goblinCreation, wolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(1), StaticCombatCommands.EquipSlashAttack(2), StaticCombatCommands.EquipSlashAttack(3));
            
            RunCombat([1], [0, 3, 2]);
            
            CombatValidator.AssertNextInitiatingCombatant(3, 2, 1, 0);
        }

        [Test]
        public void Positive_SimulateCombat_CombatClearsDown_BetweenCommands()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(1), StaticCombatCommands.EquipStabAttack(2), StaticCombatCommands.EquipStabAttack(3));
            
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = [1, 3],
                EnemyCombatantIDs = [0, 2]
            };
            
            DispatchMessage(basicEncounterDeck, basicEncounterDeck, basicEncounterDeck, basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(4);
            
            foreach (BasicEncounterDeckResponse basicEncounterDeckResponse in _responseListener.Responses)
            { 
                CombatValidator.RegisterCombatStages(basicEncounterDeckResponse.CombatStages);
                CombatValidator.AssertFirstDeadCombatant(3);
                CombatValidator.AssertNextInitiatingCombatant(2, 1, 0, 3);
            }
        }

        [Test]
        public void Positive_SimulateCombat_OnlyTheSpecifiedCombatantsFight()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(1));
            
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = [0], EnemyCombatantIDs = [1]
            };
            
            DispatchMessage(StaticCombatCommands.BearCreation);
            DispatchMessage(StaticCombatCommands.StrikeAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStrikeAttack(2));
            
            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
             
            CombatValidator.RegisterCombatStages(_responseListener.Responses[0].CombatStages);
            CombatValidator.AssertCombatantDidNotAttack(2);
        }

        [Test]
        public void Positive_SimulateCombat_DuplicateCombatantIDs_Allowed()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));
            
            BasicEncounterDeck returnedDeck = RunCombat([0, 0], [0, 0]);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], returnedDeck);
            
            CombatValidator.AssertNextInitiatingInstanceID(0, 3, 1, 2);
        }
        
        // Exception Tests
        [Test]
        public void Negative_EmptyFriendlyCombatants_DispatchesError()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation);
            BasicEncounterDeck emptyFriendlyCombatants = new() { FriendlyCombatantIDs = [], EnemyCombatantIDs = [0] };

            DispatchMessage(emptyFriendlyCombatants);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<EmptyCollectionException>(emptyFriendlyCombatants);
        }

        [Test]
        public void Negative_EmptyEnemyCombatants_DispatchesError()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation);
            BasicEncounterDeck emptyEnemyCombatants = new() { FriendlyCombatantIDs = [0], EnemyCombatantIDs = [] };
            
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
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0), StaticCombatCommands.EquipSlashAttack(0) with { CombatantID = 1 });

            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertError<MaxIterationsException>(basicEncounterDeck);

            MaxIterationsException maxIterationsException = (_errorListener.Error.BaseError.Exception.GetBaseException() as MaxIterationsException)!;
            Assert.That(maxIterationsException.MaxIterations, Is.EqualTo(maxIterations));
        }
    }
}