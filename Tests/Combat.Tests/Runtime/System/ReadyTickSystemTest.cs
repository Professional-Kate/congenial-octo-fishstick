using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class ReadyTickSystemTest
    {
        private ReadyTickSystem _readyTickSystem;
        private Mock<ICastingCalculator> _castingCalculatorMock;

        private AbilityEntity _singleStageEntity;

        private const uint COMBATANT_SPEED = 10u;
        private const double CURRENT_TICK = 100d;
        private readonly ReadyTickComponent _existingReadyTickComponent = new()
        {
            ReadyTick = CURRENT_TICK
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _castingCalculatorMock = new Mock<ICastingCalculator>();
            
            _readyTickSystem = new ReadyTickSystem(_castingCalculatorMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _singleStageEntity = TestAbilityEntityFactory.Create(1, 1);
            _singleStageEntity.AddComponent(_existingReadyTickComponent);
            
            _castingCalculatorMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _castingCalculatorMock.Verify();
            _castingCalculatorMock.VerifyNoOtherCalls();
        }

        private void SetupGetCastDuration(AbilityStage[] combatantAbilityStages)
        {
            // just returns the cast time, we don't need to mock the specific calculation
            foreach (AbilityStage combatantAbilityStage in combatantAbilityStages)
            {
                uint castTime = combatantAbilityStage.AbilityStageCards.CastTime;
                _castingCalculatorMock.Setup(library => library.GetCastDuration(COMBATANT_SPEED, castTime)).Returns(castTime).Verifiable();
            }
        }

        private static void AssertReadyTimeChanged(double expectedReadyTime, AbilityEntity changedEntity)
        {
            ReadyTickComponent readyTickComponent = changedEntity.GetComponent<ReadyTickComponent>();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(readyTickComponent.ReadyTick, Is.Not.EqualTo(CURRENT_TICK));
                Assert.That(readyTickComponent.ReadyTick, Is.EqualTo(expectedReadyTime));
            }
        }
        
        private static CooldownComponent GetCooldownComponent(AbilityEntity abilityEntity) => abilityEntity.GetComponent<CooldownComponent>();

        [Test]
        public void Positive_SetNewReadyTime_ChangesReadyTime()
        {
            Assert.DoesNotThrow(() => _readyTickSystem.SetNextReadyTick(CURRENT_TICK, _singleStageEntity, COMBATANT_SPEED));

            AssertReadyTimeChanged(CURRENT_TICK + GetCooldownComponent(_singleStageEntity).Cooldown, _singleStageEntity);
        }

        [Test]
        public void Positive_SetNewReadyTime_CurrentTickZero_ChangesReadyTime()
        {
            Assert.DoesNotThrow(() => _readyTickSystem.SetNextReadyTick(currentTick: 0, _singleStageEntity, COMBATANT_SPEED));
            
            AssertReadyTimeChanged(GetCooldownComponent(_singleStageEntity).Cooldown, _singleStageEntity);
        }

        [Test]
        public void Positive_SetNewReadyTime_AbilityHasCastTime_DelaysReadyTime()
        {
            AbilityStage[] combatantAbilities =
            [
                new()
                {
                    AbilityStageCards = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 10, MaxTargets = 1, Value = 3,
                        Priority = 0
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        CombatantStatType = CombatantStatType.HEALTH, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY
                    }
                },
                new()
                {
                    AbilityStageCards = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 20, MaxTargets = 1, Value = 3,
                        Priority = 1
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        CombatantStatType = CombatantStatType.HEALTH, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY
                    }
                },
                new()
                {
                    AbilityStageCards = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 30, MaxTargets = 1, Value = 3,
                        Priority = 2
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        CombatantStatType = CombatantStatType.HEALTH, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY
                    }
                }
            ];

            SetupGetCastDuration(combatantAbilities);
            
            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(12, 12, combatantAbilities);
            abilityEntity.AddComponent(_existingReadyTickComponent);
            
            Assert.DoesNotThrow(() => _readyTickSystem.SetNextReadyTick(currentTick: 0, abilityEntity, COMBATANT_SPEED));
            
            AssertReadyTimeChanged(GetCooldownComponent(abilityEntity).Cooldown + 60, abilityEntity);
        }
    }
}