using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class TargetingPreferenceTest : ManagedTestBuffer
    {
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            ManagedSubscribe(_responseListener);
        }

        [TearDown]
        public void TearDown()
        {
            AbilityValidator.Reset();
        }

        private void SetupCombat(TargetingPreference targetingPreference, CombatantStatType combatantStatType, CombatantCreation targetCreation)
        {
            CombatantAbilityCard mainCombatantAbilityCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { CombatantStatType = combatantStatType, TargetingPreference = targetingPreference }};
            
            DispatchMessage(targetCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.BasicAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipAbilityCards(1, mainCombatantAbilityCard));
            
            RunCombat([1], [2, 0, 3]);
            AbilityValidator.AssertTarget(0);
        }
        
        private void RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _responseListener.AssertResponseLength(1);
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
        }

        [TestCase(5000u, TargetingPreference.HIGHEST, CombatantStatType.HEALTH)]
        [TestCase(1u, TargetingPreference.LOWEST, CombatantStatType.HEALTH)]
        public void CanTarget_StatCard_Stats(uint stat, TargetingPreference targetingPreference, CombatantStatType combatantStatType)
        {
            StatCard statCard = new() { Health = stat };
            CombatantCreation targetCreation = StaticCombatCommands.HumanCreation with { StatCard = statCard };
            
            SetupCombat(targetingPreference, combatantStatType, targetCreation);
        }

        [TestCase(5000u, TargetingPreference.HIGHEST, CombatantStatType.SPEED)]
        [TestCase(1u, TargetingPreference.LOWEST, CombatantStatType.SPEED)]
        [TestCase(5000u, TargetingPreference.HIGHEST, CombatantStatType.INITIATIVE)]
        [TestCase(1u, TargetingPreference.LOWEST, CombatantStatType.INITIATIVE)]
        public void CanTarget_AgilityCard_Stats(uint stat, TargetingPreference targetingPreference, CombatantStatType combatantStatType)
        {
            AgilityCard agilityCard = combatantStatType == CombatantStatType.INITIATIVE ? new AgilityCard { Speed = 5, Initiative = stat } : new AgilityCard { Speed = stat, Initiative = 5 };
            CombatantCreation targetCreation = StaticCombatCommands.WolfCreation with { AgilityCard = agilityCard };
            
            SetupCombat(targetingPreference, combatantStatType, targetCreation);
        }
    }
}