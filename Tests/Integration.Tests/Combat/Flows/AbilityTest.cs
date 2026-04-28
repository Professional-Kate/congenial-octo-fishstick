using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class AbilityTest : ManagedTestBuffer
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
        
        private readonly AbilityCreation _strongAttackCreation = StaticCombatCommands.StrongAttackCreation; 
        private readonly CombatantAbilityEquip _equipStrongAttack = StaticCombatCommands.EquipStrongAttack(0);
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            _errorListener = new ManagedErrorListener<BasicEncounterDeckError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            _combatTools.Reset();
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
            _combatTools.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
        }

        [Test]
        public void CombatantWithNoAbilities_DoesNotAttack()
        {
            DispatchMessage(_humanCreation, _goblinCreation, _wolfCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 2 });

            RunCombat([0], [1, 2]);
            
            _combatTools.AssertZeroAttacks(_goblinCreation);
            _combatTools.AssertOneOrMoreAttacks(_humanCreation, _wolfCreation);
        }

        [TestCase(1u, false, TestName = "Enemies win because AbilityDamage is not enough to kill the one-shotting bear")]
        [TestCase(100u, true, TestName = "Friendlies win because the added AbilityDamage nukes the bear")]
        public void AbilityDamage_ShouldIncreaseDamage(uint abilityDamage, bool friendlyVictory)
        {
            CombatantCreation slightlyFasterHuman = _humanCreation with { StatCard = new StatCard { Health = 1, Attack = 1, Speed = 10 } };
            CombatantCreation slightlySlowerBear = _bearCreation with { StatCard = new StatCard { Health = 10, Attack = 100, Speed = 9 } };
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(_basicAttackCreation with { Damage = abilityDamage });
            DispatchMessage(_equipBasicAttack, _equipBasicAttack with { CombatantID = 1 });

            RunCombat([0], [1]);
            
            CombatTools.AssertVictory(_responseListener.Responses[0], friendlyVictory);
        }

        [Test]
        public void MultipleAbilities_ShouldBothBeCast_BeforeBearAttack()
        {
            AbilityCard abilityCard = new()
            {
                AbilityType = AbilityType.BASIC_ATTACK,
                StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }
            };
            
            CombatantCreation slightlyFasterHuman = _humanCreation with { StatCard = new StatCard { Health = 1, Attack = 1, Speed = 10 } };
            CombatantCreation slightlySlowerBear = _bearCreation with { StatCard = new StatCard { Health = 10, Attack = 100, Speed = 9 } };
            CombatantAbilityEquip dualAbilityEquip = new() { CombatantID = 0, AbilityCards = [ abilityCard, abilityCard with { AbilityType = AbilityType.STRONG_ATTACK } ] };
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(_basicAttackCreation with { Damage = 5 }, _strongAttackCreation with { Damage = 5, Cooldown = 1 });
            DispatchMessage(dualAbilityEquip, _equipBasicAttack with { CombatantID = 1 });

            RunCombat([0], [1]);
            
            CombatTools.AssertVictory(_responseListener.Responses[0], true);
        }
    }
}