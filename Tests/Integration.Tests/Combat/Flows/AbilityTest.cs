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
        private ManagedResponseListener<CombatantCreationResponse> _combatantCreationResponseListener;

        private readonly CombatantCreation _humanCreation = StaticCombatCommands.HumanCreation;
        private readonly CombatantCreation _goblinCreation = StaticCombatCommands.GoblinCreation;
        private readonly CombatantCreation _bearCreation = StaticCombatCommands.BearCreation;
        
        private readonly AbilityCreation _basicAttackCreation = StaticCombatCommands.SlashAttackCreation; 
        private readonly CombatantAbilityEquip _equipBasicAttack = StaticCombatCommands.EquipSlashAttack(0);
        
        private readonly AbilityCreation _strongAttackCreation = StaticCombatCommands.StabAttackCreation; 
        private readonly CombatantAbilityEquip _equipStrongAttack = StaticCombatCommands.EquipStabAttack(0);
        
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
        
        private void RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs, CombatantCreationResponse[] responses)
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
        }

        [Test]
        public void CombatantWithNoAbilities_DoesNotAttack()
        {
            DispatchMessage(_humanCreation, _goblinCreation);
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_equipBasicAttack);

            RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            _combatTools.AssertZeroAttacks(_goblinCreation);
            _combatTools.AssertOneOrMoreAttacks(_humanCreation);
        }

        [TestCase(1u, false, TestName = "Enemies win because AbilityDamage is not enough to kill the one-shotting bear")]
        [TestCase(100u, true, TestName = "Friendlies win because the added AbilityDamage nukes the bear")]
        public void AbilityDamage_ShouldIncreaseDamage(uint abilityDamage, bool friendlyVictory)
        {
            CombatantCreation slightlyFasterHuman = _humanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 4 }};
            CombatantCreation slightlySlowerBear = _bearCreation with { StatCard = new StatCard { Health = 10 }, AgilityCard = new AgilityCard { Speed = 9, Initiative = 4 }};
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(_strongAttackCreation with { ElementalDamageCard = _strongAttackCreation.ElementalDamageCard with { FireDamage = abilityDamage } });
            DispatchMessage(_equipStrongAttack, StaticCombatCommands.EquipStabAttack(1));

            RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            CombatTools.AssertVictory(_responseListener.Responses[0], friendlyVictory);
        }

        [Test]
        public void MultipleAbilities_ShouldBothBeCast_BeforeBearAttack()
        {
            CombatantAbilityCard combatantAbilityCard = new()
            {
                AbilityType = AbilityType.SLASH,
                StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }
            };
            
            CombatantCreation slightlyFasterHuman = _humanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 10 }};
            CombatantCreation slightlySlowerBear = _bearCreation with { StatCard = new StatCard { Health = 10 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 9 }};
            CombatantAbilityEquip dualAbilityEquip = new() { CombatantID = 0, AbilityCards = [ combatantAbilityCard, combatantAbilityCard with { AbilityType = AbilityType.STAB } ] };
            AbilityCard abilityCard = _strongAttackCreation.AbilityCard with { Cooldown = _basicAttackCreation.AbilityCard.Cooldown }; 
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(_basicAttackCreation with { ElementalDamageCard = _strongAttackCreation.ElementalDamageCard with { FireDamage = 5 }}, _strongAttackCreation with { ElementalDamageCard = _strongAttackCreation.ElementalDamageCard with { FireDamage = 5 }, AbilityCard  = abilityCard });
            DispatchMessage(dualAbilityEquip, StaticCombatCommands.EquipSlashAttack(1));

            RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            CombatTools.AssertVictory(_responseListener.Responses[0], true);
            
            _combatTools.AssertZeroAttacks(slightlySlowerBear);
            _combatTools.AssertOneOrMoreAttacks(slightlyFasterHuman);
            _combatTools.AssertAbilityUse(slightlyFasterHuman, combatantAbilityCard.AbilityType, 1);
            _combatTools.AssertAbilityUse(slightlyFasterHuman, AbilityType.STAB, 1);
            
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
            AbilityValidator.AssertAttackerAbility(0, AbilityType.SLASH);
            AbilityValidator.AssertAttackerAbility(0, AbilityType.STAB);
            AbilityValidator.Reset();
        }

        [Test]
        public void AbilityDamageCard_AddsToTotalDamage()
        {
            AbilityCreation abilityDamageCreation = _basicAttackCreation with
            {
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 1, FireDamage = 1, LightningDamage = 1 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 1, StrikeDamage = 1, ThrustDamage = 1 }
            };
            
            CombatantCreation elementalMan = _humanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 4 }};
            CombatantCreation unsuspectingGoblin = _goblinCreation with { StatCard = new StatCard { Health = 6 }, AgilityCard = new AgilityCard { Speed = 9, Initiative = 4 }};
            
            DispatchMessage(abilityDamageCreation);
            DispatchMessage(elementalMan, unsuspectingGoblin);
            DispatchMessage(_equipBasicAttack, StaticCombatCommands.EquipSlashAttack(1));
            
            RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            CombatTools.AssertVictory(_responseListener.Responses[0], true);
            _combatTools.AssertZeroAttacks(unsuspectingGoblin);
            _combatTools.AssertOneOrMoreAttacks(elementalMan);
        }

        [Test]
        public void AbilityWithCastTime_IsSlowerToCast()
        {
            DispatchMessage(_humanCreation, _humanCreation);
            DispatchMessage(_strongAttackCreation with { AbilityCard = _strongAttackCreation.AbilityCard with { CastTime = 10u }}, _strongAttackCreation with { AbilityCard = _strongAttackCreation.AbilityCard with { CastTime = 9u, AbilityType = AbilityType.SLASH }});
            DispatchMessage(_equipBasicAttack, StaticCombatCommands.EquipStabAttack(1));

            RunCombat([0], [1], _combatantCreationResponseListener.Responses);
            
            CombatTools.PrintStateChanges(_responseListener.Responses[0].CombatantStateChanges);
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
            AbilityValidator.AssertAttackerAbility(0, AbilityType.SLASH);
            AbilityValidator.AssertAttackerAbility(1, AbilityType.STAB);
            AbilityValidator.Reset();
        }
    }
}