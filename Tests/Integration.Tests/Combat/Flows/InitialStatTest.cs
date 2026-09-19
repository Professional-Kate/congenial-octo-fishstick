using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class InitialStatTest : ManagedTestBuffer
    {
        private ManagedResponseListener<BasicEncounterDeckResponse> _encounterDeckResponseListener;

        private StatCreation[] _statCreations;
        
        private readonly StatConfiguration _statConfiguration = new()
        {
            CombatantStatLinks = new CombatantStatLinks
            {
                BaseHealthID = 0,
                HealthID = 1,
                SpeedID = 2,
                InitiativeID = 3
            },
            AbilityStatLinks = new AbilityStatLinks
            {
                AbilityDamageID = 4,
                AbilityHealingID = 5,
                RetaliationDamageID = 6,
                CastTimeID = 7,
                CooldownID = 8,
                AbilitySlotsID = 9
            }
        };
        
        [SetUp]
        public void Setup()
        {
            // CreateStatCreations() will mutate this array so we assign it for each test
            _statCreations =
            [
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 },
                new StatCreation { InitialValue = 0 }
            ];
            
            _encounterDeckResponseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            
            ManagedSubscribe(_encounterDeckResponseListener);
        }

        private static StatCreation[] CreateStatCreations(StatType statType, uint statValue, StatCreation[] statCreations)
        {
            statCreations[(byte) statType] = new StatCreation { InitialValue = statValue };
            return statCreations;
        }

        private static ReadOnlyCombatant GetInitialCombatant(InitialEntities initialEntities, byte instanceID)
        {
            foreach (ReadOnlyCombatant readOnlyCombatant in initialEntities.FriendlyCombatants)
            {
                if (readOnlyCombatant.InstanceID == instanceID)
                {
                    return readOnlyCombatant;
                }
            }

            foreach (ReadOnlyCombatant readOnlyCombatant in initialEntities.EnemyCombatants)
            {
                if (readOnlyCombatant.InstanceID == instanceID)
                {
                    return readOnlyCombatant;
                }
            }

            throw new KeyNotFoundException();
        }

        private static ReadOnlyAbility GetInitialAbility(InitialEntities initialEntities, byte instanceID, byte abilityID)
        {
            foreach (ReadOnlyAbility initialEntitiesAbility in initialEntities.Abilities)
            {
                if (initialEntitiesAbility.AbilityID != abilityID)
                {
                    continue;
                }

                if (initialEntitiesAbility.InstanceID == instanceID)
                {
                    return initialEntitiesAbility;
                }
            }

            throw new KeyNotFoundException();
        }
        
        private void RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchMessage(basicEncounterDeck);
            
            _encounterDeckResponseListener.AssertWasCalled(true);
            _encounterDeckResponseListener.AssertResponseLength(1);
            CombatValidator.RegisterCombatStages(_encounterDeckResponseListener.Responses[0].CombatArenaLog.CombatStages);
        }


        private static IEnumerable<TestCaseData> CombatantCardCases()
        {
            yield return new TestCaseData(StatType.BASE_HEALTH, 
                new Func<ReadOnlyCombatant, uint>(initial => initial.HealthCard.BaseHealth),
                new Func<CombatantCreation, uint>(creation => creation.HealthCard.BaseHealth)).SetName("Base Health");
            
            yield return new TestCaseData(StatType.HEALTH, 
                new Func<ReadOnlyCombatant, uint>(initial => initial.HealthCard.Health),
                new Func<CombatantCreation, uint>(creation => creation.HealthCard.Health)).SetName("Health");
            
            yield return new TestCaseData(StatType.SPEED, 
                new Func<ReadOnlyCombatant, uint>(initial => initial.AgilityCard.Speed),
                new Func<CombatantCreation, uint>(creation => creation.AgilityCard.Speed)).SetName("Speed");
            
            yield return new TestCaseData(StatType.INITIATIVE, 
                new Func<ReadOnlyCombatant, uint>(initial => initial.AgilityCard.Initiative),
                new Func<CombatantCreation, uint>(creation => creation.AgilityCard.Initiative)).SetName("Initiative");
        }

        [TestCaseSource(nameof(CombatantCardCases))]
        public void InitialCombatantStats(StatType statType, Func<ReadOnlyCombatant, uint> readonlyStatGetter, Func<CombatantCreation, uint> creationStatGetter)
        {
            DispatchMessage(CreateStatCreations(statType, 100, _statCreations));
            DispatchMessage(_statConfiguration);
            DispatchMessage(StaticCombatCommands.HumanCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));
            
            RunCombat([0], [0]);

            ReadOnlyCombatant readOnlyCombatant = GetInitialCombatant(_encounterDeckResponseListener.Responses[0].CombatArenaLog.InitialEntities, 0);
            Assert.That(readonlyStatGetter(readOnlyCombatant), Is.EqualTo(100 + creationStatGetter(StaticCombatCommands.HumanCreation)));
        }
        
        private static IEnumerable<TestCaseData> AbilityCardCases()
        {
            yield return new TestCaseData(StatType.ABILITY_SLOTS, 
                new Func<ReadOnlyAbility, uint>(initial => initial.AbilityCard.AbilitySlots),
                new Func<AbilityCreation, uint>(creation => creation.AbilityCard.AbilitySlots)).SetName("Ability Slots");
            
            yield return new TestCaseData(StatType.COOLDOWN, 
                new Func<ReadOnlyAbility, uint>(initial => initial.AbilityCard.Cooldown),
                new Func<AbilityCreation, uint>(creation => creation.AbilityCard.Cooldown)).SetName("Cooldown");
        }
        
        [TestCaseSource(nameof(AbilityCardCases))]
        public void InitialAbilityStats(StatType statType, Func<ReadOnlyAbility, uint> readonlyStatGetter, Func<AbilityCreation, uint> creationStatGetter)
        {
            DispatchMessage(CreateStatCreations(statType, 100, _statCreations));
            DispatchMessage(_statConfiguration);
            DispatchMessage(StaticCombatCommands.HumanCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));
            
            RunCombat([0], [0]);

            ReadOnlyAbility readonlyAbility = GetInitialAbility(_encounterDeckResponseListener.Responses[0].CombatArenaLog.InitialEntities, 0, 0);
            Assert.That(readonlyStatGetter(readonlyAbility), Is.EqualTo(100 + creationStatGetter(StaticCombatCommands.SlashAttackCreation)));
        }

        [TestCase(AbilityEffectType.DIRECT_DAMAGE, StatType.ABILITY_DAMAGE, TestName = "Damage")]
        [TestCase(AbilityEffectType.HEALING, StatType.ABILITY_HEALING, TestName = "Healing")]
        [TestCase(AbilityEffectType.RETALIATION, StatType.RETALIATION_DAMAGE, TestName = "Retaliation")]
        public void AbilityStageStats(AbilityEffectType abilityEffectType, StatType statType)
        {
            AbilityCreation allStageCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 10 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = 
                [
                    new AbilityStageCard { AbilityEffectType = abilityEffectType, AffinityType = AffinityType.FIRE, CastTime = 10, MaxTargets = 1, Value = 10, Priority = 0 },
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 10, MaxTargets = 1, Value = 100, Priority = 1 }
                ]
            };
            
            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [new EquippedAbility { AbilityID = 0, StrategyCards = 
                [
                    new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY, StatType = StatType.HEALTH, Priority = 0 },
                    new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY, StatType = StatType.HEALTH, Priority = 1 }
                ]}]
            };
            
            DispatchMessage(CreateStatCreations(statType, 100, _statCreations));
            DispatchMessage(_statConfiguration);
            DispatchMessage(StaticCombatCommands.HumanCreation);
            DispatchMessage(allStageCreation);
            DispatchMessage(abilityEquip);
            
            RunCombat([0], [0]);
            
            ReadOnlyAbility readonlyAbility = GetInitialAbility(_encounterDeckResponseListener.Responses[0].CombatArenaLog.InitialEntities, 0, 0);
            Assert.That(readonlyAbility.ReadOnlyAbilityStages, Has.Length.EqualTo(2));
            Assert.That(readonlyAbility.ReadOnlyAbilityStages[0].Value, Is.EqualTo(110));
        }

        [Test]
        public void CastTime()
        {
            AbilityCreation allStageCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 10 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = 
                [
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Value = 10, Priority = 0 }
                ]
            };
            
            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [new EquippedAbility { AbilityID = 0, StrategyCards = 
                [
                    new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY, StatType = StatType.HEALTH, Priority = 0 }
                ]}]
            };
            
            DispatchMessage(CreateStatCreations(StatType.CAST_TIME, 100, _statCreations));
            DispatchMessage(_statConfiguration);
            DispatchMessage(StaticCombatCommands.HumanCreation);
            DispatchMessage(allStageCreation);
            DispatchMessage(abilityEquip);
            
            RunCombat([0], [0]);
            
            ReadOnlyAbility readonlyAbility = GetInitialAbility(_encounterDeckResponseListener.Responses[0].CombatArenaLog.InitialEntities, 0, 0);
            Assert.That(readonlyAbility.ReadOnlyAbilityStages[0].CastTime, Is.EqualTo(100));
        }
    }
}