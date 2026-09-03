using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Event;
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
            CombatValidator.Reset();
        }

        private void SetupCombat(TargetingPreference targetingPreference, CombatantStatType combatantStatType, CombatantCreation targetCreation)
        {
            EquippedAbility mainEquippedAbility = new() { AbilityID = 0, StrategyCards = [ new StrategyCard { CombatantStatType = combatantStatType, TargetingPreference = targetingPreference, TargetingType = TargetingType.ENEMY, Priority = 0 }]};

            DispatchMessage(targetCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipAbilityCards(1, mainEquippedAbility));
            
            RunCombat([1], [2, 0, 3]);
            // CombatValidator.PrintCombatStages(_responseListener.Responses[0].CombatStages);
            CombatValidator.AssertNextTargets(0);
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
            CombatValidator.RegisterCombatStages(_responseListener.Responses[0].CombatStages);
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

        [TestCase(TargetingPreference.HIGHEST, AbilityEffectType.DIRECT_DAMAGE)]
        [TestCase(TargetingPreference.LOWEST, AbilityEffectType.DIRECT_DAMAGE)]
        [TestCase(TargetingPreference.HIGHEST, AbilityEffectType.HEALING)]
        [TestCase(TargetingPreference.LOWEST, AbilityEffectType.HEALING)]
        public void CanTarget_AbilityStats(TargetingPreference targetingPreference, AbilityEffectType abilityEffectType)
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation with { StatCard = new StatCard { Health = 1 }}, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation);

            uint abilityDamage = targetingPreference == TargetingPreference.HIGHEST ? uint.MaxValue : uint.MinValue;
            AbilityCreation highDamageAbility = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger with { TriggerEventType = TriggerEventType.COMBATANT_CASTING_COMPLETE, TargetingType = TargetingType.ENEMY },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = abilityEffectType, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = abilityDamage, Priority = 0 }]
            };
            
            // Equipping enemy combatant with the high damage ability
            DispatchMessage(highDamageAbility);
            DispatchMessage(new AbilityEquip { CombatantID = 1, EquippedAbilities = [new EquippedAbility {AbilityID = 0, StrategyCards = [new StrategyCard
            {
                CombatantStatType = CombatantStatType.HEALTH,
                TargetingPreference = TargetingPreference.HIGHEST,
                TargetingType = TargetingType.ENEMY,
                Priority = 0
            }]}]});
            
            // Equipping our friendly combatant with the expected Strategy
            CombatantStatType combatantStatType = abilityEffectType == AbilityEffectType.DIRECT_DAMAGE ? CombatantStatType.ABILITY_DAMAGE : CombatantStatType.ABILITY_HEALING;
            
            DispatchMessage(StaticCombatCommands.StabAttackCreation);
            EquippedAbility highDamageTargeting = new() { AbilityID = 1, StrategyCards = [new StrategyCard { CombatantStatType = combatantStatType, TargetingPreference = targetingPreference, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            DispatchMessage(new AbilityEquip { CombatantID = 0, EquippedAbilities = [highDamageTargeting]});
            
            // Equipping other enemies with abilities to verify ability values
            AbilityCreation healingAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.HOLY, CastTime = 0, MaxTargets = 1, Value = 3, Priority = 0 }]
            };
            
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, healingAbilityCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(2, 2), StaticCombatCommands.EquipAbility(3, 3));
            
            RunCombat([0], [1, 2, 3]);
            CombatValidator.AssertFirstDeadCombatant(1);
        }
    }
}