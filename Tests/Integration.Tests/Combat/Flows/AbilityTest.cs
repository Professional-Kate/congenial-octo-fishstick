using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Logging;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class AbilityTest : ManagedTestBuffer
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

        [Test]
        public void CombatantWithNoAbilities_DoesNotAttack()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));

            RunCombat([0], [1]);

            CombatValidator.AssertCombatantDidNotAttack(1);
            CombatValidator.AssertNextInitiatingCombatant(0);
        }

        [TestCase(1u, false, TestName = "Enemies win because AbilityDamage is not enough to kill the one-shotting bear")]
        [TestCase(100u, true, TestName = "Friendlies win because the added AbilityDamage nukes the bear")]
        public void AbilityDamage_ShouldIncreaseDamage(uint abilityDamage, bool friendlyVictory)
        {
            CombatantCreation slightlyFasterHuman = StaticCombatCommands.HumanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 4 }};
            CombatantCreation slightlySlowerBear = StaticCombatCommands.BearCreation with { StatCard = new StatCard { Health = 10 }, AgilityCard = new AgilityCard { Speed = 9, Initiative = 4 }};
            
            AbilityCreation slashAttackCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = abilityDamage, Priority = 0 } ]
            };
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(StaticCombatCommands.StabAttackCreation, slashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStabAttack(0), StaticCombatCommands.EquipStabAttack(1));

            RunCombat([0], [1]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], friendlyVictory);
        }

        [Test]
        public void MultipleAbilities_ShouldBothBeCast_BeforeBearAttack()
        {
            EquippedAbility equippedAbility = new()
            {
                AbilityID = 0,
                StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]
            };
            
            CombatantCreation slightlyFasterHuman = StaticCombatCommands.HumanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 10 }};
            CombatantCreation slightlySlowerBear = StaticCombatCommands.BearCreation with { StatCard = new StatCard { Health = 10 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 9 }};
            AbilityEquip dualAbilityEquip = new() { CombatantID = 0, EquippedAbilities = [ equippedAbility, equippedAbility with { AbilityID = 1 } ] };
            
            DispatchMessage(slightlyFasterHuman, slightlySlowerBear);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(dualAbilityEquip, StaticCombatCommands.EquipSlashAttack(1));
        
            RunCombat([0], [1]);
            
            CombatValidator.AssertNextInitiatingCombatant(0, 0, 1);
        }

        [Test]
        public void MultipleStages_AddsToTotalDamage()
        {
            AbilityCreation multipleStagesCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = 
                    [ 
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 3, Priority = 123 },
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STAB, CastTime = 0,  MaxTargets = 1, Value = 3, Priority = 12 }
                    ]
            };

            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [new EquippedAbility { AbilityID = 0, StrategyCards = 
                    [
                        new StrategyCard { TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 12 },
                        new StrategyCard { TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 123 }
                    ]}]
            };
            
            CombatantCreation elementalMan = StaticCombatCommands.HumanCreation with { StatCard = new StatCard { Health = 1 }, AgilityCard = new AgilityCard { Speed = 10, Initiative = 4 }};
            CombatantCreation unsuspectingGoblin = StaticCombatCommands.GoblinCreation with { StatCard = new StatCard { Health = 6 }, AgilityCard = new AgilityCard { Speed = 9, Initiative = 4 }};
            
            DispatchMessage(multipleStagesCreation);
            DispatchMessage(elementalMan, unsuspectingGoblin);
            DispatchMessage(abilityEquip);
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], true);
            CombatValidator.AssertCombatantDidNotAttack(1);
            CombatValidator.AssertNextInitiatingCombatant(0);
        }

        [Test]
        public void AbilityWithCastTime_IsSlowerToCast()
        {
            AbilityCreation abilityWithCastTime = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 1, MaxTargets = 1, Value = 3, Priority = 0 }]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.HumanCreation);
            DispatchMessage(abilityWithCastTime, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, 0), StaticCombatCommands.EquipAbility(1, 1));

            RunCombat([0], [1]);
            
            CombatValidator.AssertNextAbilityID(1, 0);
        }

        [Test]
        public void DifferentInitiative_AbilityWithMultipleStages_OnlySomeStagesCast_BeforeDeath()
        {
            AbilityCreation abilityWithCastTime = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = 
                [ 
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 1, Priority = 0 },
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 100, MaxTargets = 1, Value = 200, Priority = 1 }
                ]
            };

            AbilityEquip equipCastTimeAbility = new()
            {
                CombatantID = 0,
                EquippedAbilities =
                [
                    new EquippedAbility
                    {
                        AbilityID = 0, StrategyCards =
                        [
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 },
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 1 }
                        ]
                    }
                ]
            };

            DispatchMessage(StaticCombatCommands.GoblinCreation with { StatCard = new  StatCard { Health = 1 }}, StaticCombatCommands.HumanCreation);
            DispatchMessage(abilityWithCastTime, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(equipCastTimeAbility, StaticCombatCommands.EquipAbility(1, 1));
            
            RunCombat([1], [0]);
            
            CombatValidator.AssertNextInitiatingCombatant(0, 1);
        }

        [Test]
        public void HealingAbility_HealsAlly()
        {
            AbilityStageCard healingCard = new() { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.HOLY, CastTime = 0,  MaxTargets = 1, Value = 1, Priority = 0 };
            
            AbilityCreation healingAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [healingCard]
            };

            AbilityEquip healingAbilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities =
                [
                    new EquippedAbility
                    {
                        AbilityID = 1,
                        StrategyCards =
                        [
                            new StrategyCard
                            {
                                TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH,
                                TargetingType = TargetingType.FRIENDLY, Priority = 0
                            }
                        ]
                    }
                ]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.BearCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation, StaticCombatCommands.StabAttackCreation, healingAbilityCreation);
            DispatchMessage( StaticCombatCommands.EquipSlashAttack(1), StaticCombatCommands.EquipStabAttack(2));
            
            RunCombat([0, 1], [2]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], false);
            CombatValidator.AssertFirstDeadCombatant(0);
            CombatValidator.Reset();
            
            // after equipping the healing ability, the allies should win!
            DispatchMessage(healingAbilityEquip);
            
            RunCombat([0, 1], [2]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], false);
            CombatValidator.AssertFirstDeadCombatant(1);
        }

        [Test]
        public void RetaliationAbility_OnlyActivatedAfterBeingDamaged()
        {
            AbilityStageCard healingCard = new() { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 1, Value = 1, Priority = 0 };
            AbilityStageCard retaliationCard = new() { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 1, Value = 16, Priority = 1 };
            
            AbilityCreation retaliationAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [healingCard, retaliationCard]
            };

            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0, 
                EquippedAbilities = 
                [
                    new EquippedAbility
                    {
                        AbilityID = 0, 
                        StrategyCards = 
                        [ 
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.FRIENDLY, Priority = 0 },
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 1 }
                        ]
                    }
                ]
            };

            DispatchMessage(StaticCombatCommands.HumanCreation with { AgilityCard = new AgilityCard { Speed  = 20, Initiative = 5 }}, StaticCombatCommands.GoblinCreation);
            DispatchMessage(retaliationAbilityCreation, StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(abilityEquip, StaticCombatCommands.EquipAbility(1, 1));
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertFirstDeadCombatant(1);
            
            // 0 - Human heals self (AbilityStage 1/2)
            // 1 - Wolf attacks (Human couldn't cast Retaliation)
            // 0 - Human heals self, and uses Retaliation on wolf (AbilityStage 1/2 & 2/2)
            CombatValidator.AssertNextInitiatingCombatant(0, 1, 0);
        }

        [Test]
        public void RetaliationAbility_HitsEveryEnemy()
        {
            AbilityStageCard retaliationCard = new() { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 5, Value = 16, Priority = 0 };
            
            AbilityCreation retaliationAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [retaliationCard]
            };

            CombatantCreation fastGoblin = StaticCombatCommands.GoblinCreation with { AgilityCard =  new AgilityCard { Speed  = 20, Initiative = 5 }};
            
            DispatchMessage(StaticCombatCommands.HumanCreation, fastGoblin, fastGoblin, fastGoblin, fastGoblin, fastGoblin);
            DispatchMessage(retaliationAbilityCreation, StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, 0), StaticCombatCommands.EquipAbility(1, 1), StaticCombatCommands.EquipAbility(2, 1), StaticCombatCommands.EquipAbility(3, 1), StaticCombatCommands.EquipAbility(4, 1), StaticCombatCommands.EquipAbility(5, 1));
            
            RunCombat([0], [1, 2, 3, 4, 5]);
            
            CombatValidator.AssertNextInitiatingCombatant(1, 5, 2, 3, 4, 0);
            
            // validating that all enemies die in one attack
            CombatStage lastStageChange = _responseListener.Responses[0].CombatStages[^1];
            Assert.That(lastStageChange.CombatantStateChanges, Has.Length.EqualTo(1));
            Assert.That(lastStageChange.CombatantStateChanges[0].TargetCombatants, Has.Length.EqualTo(5));
        }

        [Test]
        public void RetaliationAbility_CannotHitSeLf()
        {
            AbilityStageCard retaliationCard = new() { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 5, Value = 3, Priority = 0 };
            
            AbilityCreation retaliationAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [retaliationCard]
            };

            AbilityEquip selfTargetAbilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities =
                [
                    new EquippedAbility
                    {
                        AbilityID = 0,
                        StrategyCards = 
                            [
                                new StrategyCard
                                {
                                    TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH,
                                    TargetingType = TargetingType.ENEMY, Priority = 0
                                }
                            ]
                    },
                    new EquippedAbility
                    {
                        AbilityID = 1,
                        StrategyCards =
                        [
                            new StrategyCard
                            {
                                TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH,
                                TargetingType = TargetingType.FRIENDLY, Priority = 0
                            }
                        ]
                    }
                ]
            };

            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.BearCreation);
            DispatchMessage(retaliationAbilityCreation, StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(selfTargetAbilityEquip);
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertAbilityNeverUsed(0);
            CombatValidator.AssertFirstDeadCombatant(0);
        }

        [Test]
        public void RetaliationAbility_ManyEnemyHits_DoesNotOverkill()
        {
            AbilityStageCard retaliationCard = new() { AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 7, Value = 10, Priority = 0 };
            
            AbilityCreation retaliationAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 5, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [retaliationCard]
            };
            
            AbilityStageCard stabStage = new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 1, Value = 1, Priority = 0 };
            AbilityCreation englandAbilityCreation = StaticCombatCommands.StabAttackCreation with
            {
                AbilityStageCards = [stabStage, stabStage, stabStage, stabStage, stabStage, stabStage, stabStage]
            };
            
            StrategyCard enemyTargeting = new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 };
            AbilityEquip stabEquip = new()
            {
                CombatantID = 1,
                EquippedAbilities =
                [
                    new EquippedAbility
                    {
                        AbilityID = 1,
                        StrategyCards = [enemyTargeting, enemyTargeting, enemyTargeting, enemyTargeting, enemyTargeting, enemyTargeting, enemyTargeting]
                    }
                ]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(retaliationAbilityCreation, englandAbilityCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, 0), stabEquip);
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertNextAbilityID(1, 0);
        }
    }
}