using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class SpecificAbilityTest : ManagedTestBuffer
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

        private static uint GetExpectedDamage(AbilityCreation abilityCreation)
        {
            uint totalDamage = 0;
            foreach (AbilityStageCard abilityStage in abilityCreation.AbilityStageCards)
            {
                totalDamage += abilityStage.Value;
            }

            return totalDamage;
        }

        private static void AssertDamageDealt(AbilityCreation abilityCreation, byte abilityID, byte targetID = 1)
        {
            CombatStage combatantStateChange = CombatValidator.GetCombatStage();

            uint damageDealt = 0;
            foreach (CombatantStateChange stateChange in combatantStateChange.CombatantStateChanges)
            {
                damageDealt += stateChange.ReadOnlyAbilityStage.Value;
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantStateChange.AbilityID, Is.EqualTo(abilityID));
                Assert.That(combatantStateChange.CombatantStateChanges[0].TargetCombatants[0].InstanceID, Is.EqualTo(targetID));
                Assert.That(damageDealt, Is.EqualTo(GetExpectedDamage(abilityCreation)));
            }
        }
        
        [Test]
        public void SlashAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation with { StatCard = new StatCard { Health = 1 }});
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.SlashAttackCreation, 0);
        }

        [Test]
        public void StabAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation with { StatCard = new StatCard { Health = 1 }});
            DispatchMessage(StaticCombatCommands.StabAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStabAttack(0, 0));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.StabAttackCreation, 0);
        }

        [Test]
        public void StrikeAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation with { StatCard = new StatCard { Health = 1 }});
            DispatchMessage(StaticCombatCommands.StrikeAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStrikeAttack(0, 0));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.StrikeAttackCreation, 0);
        }

        [Test]
        public void CombatantDamagedTrigger_ActivatesAbilityOnDamage()
        {
            AbilityCreation triggerAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 0, MaxTriggerValue = 10 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Value = 100, Priority = 0 }]
            };

            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [ new EquippedAbility { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 0 }]}]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(triggerAbilityCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(abilityEquip, StaticCombatCommands.EquipStabAttack(1));
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], friendlyVictory: true);
            CombatValidator.AssertNextInitiatingCombatant(1, 0);
        }
        
        [Test]
        public void CombatantDeathTrigger_ActivatesAbilityOnDamage()
        {
            AbilityCreation triggerAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 0, MaxTriggerValue = 10 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Value = 100, Priority = 0 }]
            };

            StrategyCard lowHealthStrategyCard = new()
            {
                TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 0
            };
            
            AbilityEquip equipDeathTrigger = new()
            {
                CombatantID = 0,
                EquippedAbilities = [ new EquippedAbility { AbilityID = 0, StrategyCards = [lowHealthStrategyCard]}]
            };

            AbilityEquip equipStabAttack = new()
            {
                CombatantID = 1,
                EquippedAbilities = [new EquippedAbility { AbilityID = 1, StrategyCards = [lowHealthStrategyCard] }]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation, StaticCombatCommands.GoblinCreation with { StatCard = new StatCard { Health = 1 }});
            DispatchMessage(triggerAbilityCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(equipDeathTrigger, equipStabAttack);
            
            RunCombat([0, 2], [1]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], friendlyVictory: true);
            CombatValidator.AssertNextInitiatingCombatant(1, 0);
        }

        [Test]
        public void CombatantCastingTrigger_TriggersAbility_BeforeOriginalActivates()
        {
            AbilityCreation triggerAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_CASTING_COMPLETE, TargetingType = TargetingType.ENEMY, MinTriggerValue = 0, MaxTriggerValue = 10 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Value = 100, Priority = 0 }]
            };
            
            AbilityCreation castingAbilityCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 5 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 1, MaxTargets = 1, Value = 100, Priority = 0 }]
            };

            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(triggerAbilityCreation, castingAbilityCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, 0), StaticCombatCommands.EquipAbility(1, 1));
            
            RunCombat([0], [1]);
            
            CombatValidator.AssertVictory(_responseListener.Responses[0], friendlyVictory: true);
            CombatValidator.AssertNextInitiatingCombatant(0);
        }

        [Test]
        public void TriggerAbility_CanTriggerFromTrigger()
        {
            AbilityCreation combatantDamagedTriggerCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.ENEMY, MinTriggerValue = 0, MaxTriggerValue = 10 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 200, Priority = 0 }]
            };
            
            AbilityCreation combatantDiedTriggerCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 2, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.ENEMY, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0, MaxTargets = 1, Value = 100, Priority = 0 }]
            };
            
            StrategyCard lowHealthCard = new()
            {
                TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 0
            };

            AbilityEquip equipTriggers = new()
            {
                CombatantID = 0,
                EquippedAbilities = [ new EquippedAbility { AbilityID = 0, StrategyCards = [lowHealthCard]}, new EquippedAbility { AbilityID = 1, StrategyCards = [lowHealthCard]}]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation, StaticCombatCommands.GoblinCreation with { StatCard = new StatCard { Health = 100 }}, StaticCombatCommands.BearCreation with { StatCard = new StatCard { Health = 100 }});
            DispatchMessage(combatantDamagedTriggerCreation, combatantDiedTriggerCreation, StaticCombatCommands.StabAttackCreation);
            DispatchMessage(equipTriggers, StaticCombatCommands.EquipAbility(1, 2));
            
            RunCombat([0, 1], [2, 3]);
            CombatValidator.AssertNextInitiatingCombatant(1, 0, 0);
        }

        [Test]
        public void ReadyTimeComponent_PreventsMultipleActivationsDuringCooldown()
        {
            AbilityCreation combatantDamagedCreation = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 5 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 0, MaxTriggerValue = 10 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 50, Priority = 0 }]
            };
            
            AbilityEquip triggerEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [ new EquippedAbility { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingType = TargetingType.ENEMY, TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH, Priority = 0 }]}]
            };
            
            AbilityCreation multiStageAbility = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 3, Cooldown = 5 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = 
                [
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 1, Value = 5, Priority = 0 },
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 5, Priority = 1 }
                ]
            };
            
            AbilityEquip multiStageAbilityEquip = new()
            {
                CombatantID = 1,
                EquippedAbilities =
                [
                    new EquippedAbility { AbilityID = 1, StrategyCards = 
                        [
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 },
                            new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 1 }
                        ]}
                ]
            };
            
            // Human will take 3 hits to kill, wolf will take 2. Wolf will attack twice in one ability, this should only let the human attack once back before he dies. 
            DispatchMessage(StaticCombatCommands.HumanCreation with { StatCard = new StatCard { Health = 11 }}, StaticCombatCommands.WolfCreation with { StatCard = new StatCard { Health = 51 }});
            DispatchMessage(combatantDamagedCreation, multiStageAbility);
            DispatchMessage(triggerEquip, multiStageAbilityEquip);
            
            RunCombat([0], [1]);
            
            // 1 - Wolf Attack (AbilityStage 1/2)
            // 0 - Human Combatant Damaged Trigger
            // 1 - Wolf finishes attack (AbilityStage 2/2)
            // 1 - Wolf attacks again (AbilityStage 1/2)
            CombatValidator.AssertNextInitiatingCombatant(1, 0, 1, 1);
        }

        [Test]
        public void MultipleStages_AllStagesComplete()
        {
            AbilityCreation multiStageAbility = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 3, Cooldown = 30 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = 
                    [
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 3, Value = 2, Priority = 2 },
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 3, Value = 4, Priority = 0 },
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.HOLY, CastTime = 0, MaxTargets = 4, Value = 1, Priority = 3 },
                        new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.HOLY, CastTime = 0, MaxTargets = 1, Value = 4, Priority = 1 }
                    ]
            };

            StrategyCard[] strategyCards =
            [
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.FRIENDLY, Priority = 2 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.INITIATIVE, TargetingType = TargetingType.ENEMY, Priority = 1 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 },
                new() { TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.SPEED, TargetingType = TargetingType.FRIENDLY, Priority = 3 }
            ];
            
            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities =
                [
                    new EquippedAbility { AbilityID = 0, StrategyCards = strategyCards }
                ]
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation, StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation, StaticCombatCommands.BearCreation, StaticCombatCommands.GoblinCreation);
            DispatchMessage(multiStageAbility);
            DispatchMessage(abilityEquip, abilityEquip with { CombatantID = 1 }, abilityEquip with { CombatantID = 2 }, abilityEquip with { CombatantID = 3 }, abilityEquip with { CombatantID = 4 }, abilityEquip with { CombatantID = 5 }, abilityEquip with { CombatantID = 6 }, abilityEquip with { CombatantID = 7 });
            
            RunCombat(friendlyCombatantIDs: [0, 1, 4, 5], enemyCombatantIDs: [2, 3, 6, 7]);
            
            CombatValidator.AssertCombatantDidAttack(0, 1, 2, 3, 4, 5, 6, 7);
        }
    }
}