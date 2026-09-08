using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class TargetingPreferenceTest : ManagedTestBuffer
    {
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;

        private readonly AbilityStageCard _oneShotCard = new()
        {
            AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, MaxTargets = 1, Priority = 0, CastTime = 0, Value = 5000
        };

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

        private void SetupCombatantStatTargeting(TargetingPreference targetingPreference, StatType statType, CombatantCreation targetCreation)
        {
            EquippedAbility mainEquippedAbility = new() { AbilityID = 0, StrategyCards = [ new StrategyCard { StatType = statType, TargetingPreference = targetingPreference, TargetingType = TargetingType.ENEMY, Priority = 0 }]};

            AbilityCreation abilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 2 },
                TriggerCard = new TriggerCard { TargetingType = TargetingType.SELF, TriggerEventType = TriggerEventType.ABILITY_READY, MaxTriggerValue = 0, MinTriggerValue = 0 }, 
                AbilityStageCards = [_oneShotCard]
            };

            DispatchMessage(targetCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(abilityCreation);
            DispatchMessage(StaticCombatCommands.EquipAbilityCards(1, mainEquippedAbility));
            
            RunCombat([1], [2, 0, 3]);
            CombatValidator.AssertCombatantDiedFirst(0);
        }

        private void SetupAbilityStatTargeting(TargetingPreference targetingPreference, StatType statType, AbilityCreation targetAbilityCreation)
        {
            EquippedAbility mainEquippedAbility = new() { AbilityID = 0, StrategyCards = [ new StrategyCard { StatType = statType, TargetingPreference = targetingPreference, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            
            AbilityCreation abilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 2 },
                TriggerCard = new TriggerCard { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.COMBATANT_DEATH, MaxTriggerValue = 0, MinTriggerValue = 0 },
                AbilityStageCards = [_oneShotCard]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, targetAbilityCreation, abilityCreation);
            DispatchMessage(StaticCombatCommands.EquipAbilityCards(1, mainEquippedAbility), StaticCombatCommands.EquipAbility(0, 1), StaticCombatCommands.EquipAbility(2, 2), StaticCombatCommands.EquipAbility(3, 2));
            
            RunCombat([1], [2, 0, 3]);
            CombatValidator.AssertCombatantDiedFirst(0);
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

        [TestCase(5000u, TargetingPreference.HIGHEST, StatType.HEALTH)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.HEALTH)]
        [TestCase(5000u, TargetingPreference.HIGHEST, StatType.BASE_HEALTH)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.BASE_HEALTH)]
        public void CanTarget_HealthCard_Stats(uint stat, TargetingPreference targetingPreference, StatType statType)
        {
            HealthCard healthCard = new() { Health = stat, BaseHealth = stat };
            
            CombatantCreation targetCreation = StaticCombatCommands.HumanCreation with { HealthCard = healthCard };
            
            SetupCombatantStatTargeting(targetingPreference, statType, targetCreation);
        }

        [TestCase(5000u, TargetingPreference.HIGHEST, StatType.SPEED)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.SPEED)]
        [TestCase(5000u, TargetingPreference.HIGHEST, StatType.INITIATIVE)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.INITIATIVE)]
        public void CanTarget_AgilityCard_Stats(uint stat, TargetingPreference targetingPreference, StatType statType)
        {
            AgilityCard agilityCard = statType == StatType.INITIATIVE ? new AgilityCard { Speed = 5, Initiative = stat } : new AgilityCard { Speed = stat, Initiative = 5 };
            CombatantCreation targetCreation = StaticCombatCommands.WolfCreation with { AgilityCard = agilityCard };
            
            SetupCombatantStatTargeting(targetingPreference, statType, targetCreation);
        }
        
        [TestCase(5000u, TargetingPreference.HIGHEST, StatType.COOLDOWN)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.COOLDOWN)]
        [TestCase(3u, TargetingPreference.HIGHEST, StatType.ABILITY_SLOTS)]
        [TestCase(1u, TargetingPreference.LOWEST, StatType.ABILITY_SLOTS)]
        public void CanTarget_AbilityCard_Stats(uint stat, TargetingPreference targetingPreference, StatType statType)
        {
            AbilityCard targetAbilityCard = statType == StatType.COOLDOWN
                ? new AbilityCard { Cooldown = stat, AbilitySlots = 1 }
                : new AbilityCard { Cooldown = 15, AbilitySlots = stat };
            
            AbilityCreation targetAbilityCreation = new()
            {
                AbilityCard = targetAbilityCard,
                TriggerCard = new TriggerCard { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.COMBATANT_DEATH, MaxTriggerValue = 0, MinTriggerValue = 0 },
                AbilityStageCards = StaticCombatCommands.SlashAttackCreation.AbilityStageCards
            };

            SetupAbilityStatTargeting(targetingPreference, statType, targetAbilityCreation);
        }

        [TestCase(TargetingPreference.HIGHEST, AbilityEffectType.DIRECT_DAMAGE)]
        [TestCase(TargetingPreference.LOWEST, AbilityEffectType.DIRECT_DAMAGE)]
        [TestCase(TargetingPreference.HIGHEST, AbilityEffectType.HEALING)]
        [TestCase(TargetingPreference.LOWEST, AbilityEffectType.HEALING)]
        [TestCase(TargetingPreference.HIGHEST, AbilityEffectType.RETALIATION)]
        [TestCase(TargetingPreference.LOWEST, AbilityEffectType.RETALIATION)]
        public void CanTarget_AbilityEffectType_Stats(TargetingPreference targetingPreference, AbilityEffectType abilityEffectType)
        {
            // Using "1" here ensures this ability is preferred over combatants with a stat value of 0.
            // See CombatantTargetFinder.GetPriority documentation for this behaviour.
            uint targetAbilityValue = targetingPreference == TargetingPreference.HIGHEST ? uint.MaxValue : 1;
            AbilityCreation targetAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = new TriggerCard { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.COMBATANT_DEATH, MaxTriggerValue = 0, MinTriggerValue = 0 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = abilityEffectType, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = targetAbilityValue, Priority = 0 }]
            };

            StatType statType = abilityEffectType switch
            {
                AbilityEffectType.DIRECT_DAMAGE => StatType.ABILITY_DAMAGE,
                AbilityEffectType.HEALING => StatType.ABILITY_HEALING,
                AbilityEffectType.RETALIATION => StatType.RETALIATION_DAMAGE,
                _ => throw new ArgumentOutOfRangeException(nameof(abilityEffectType), abilityEffectType, null)
            }; 
            SetupAbilityStatTargeting(targetingPreference, statType, targetAbilityCreation);
        }

        [TestCase(TargetingPreference.HIGHEST)]
        [TestCase(TargetingPreference.LOWEST)]
        public void CanTarget_CastTime(TargetingPreference targetingPreference)
        {
            uint castTime = targetingPreference == TargetingPreference.HIGHEST ? uint.MaxValue : 1;
            AbilityCreation targetAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = new TriggerCard { TargetingType = TargetingType.ENEMY, TriggerEventType = TriggerEventType.COMBATANT_DEATH, MaxTriggerValue = 0, MinTriggerValue = 0 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = castTime, MaxTargets = 1, Value = 1, Priority = 0 }]
            };
            
            SetupAbilityStatTargeting(targetingPreference, StatType.CAST_TIME, targetAbilityCreation);
        }
    }
}