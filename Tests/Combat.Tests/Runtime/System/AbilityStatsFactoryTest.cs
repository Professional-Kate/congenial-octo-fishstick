using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityStatsFactoryTest
    {
        private AbilityStatsFactory _abilityStatsFactory;
        private Mock<IStatComponentFactory> _statComponentFactoryMock;

        private readonly (StatType StatType, uint LinkedStatValue)[] _linkedStats =
        [
            (StatType.ABILITY_SLOTS, 1),
            (StatType.COOLDOWN, 2),
            (StatType.CAST_TIME, 6),
            (StatType.ABILITY_DAMAGE, 10),
            (StatType.ABILITY_HEALING, 5),
            (StatType.RETALIATION_DAMAGE, 15)
        ];
        
        private readonly AbilityStagesComponent _abilityStagesComponent = new()
        {
            AbilityStages =
            [
                new AbilityStage
                {
                    AbilityStageCard = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, MaxTargets = 1, Value = 10, CastTime = 1, Priority = 0
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        StatType = StatType.COOLDOWN, TargetingPreference = TargetingPreference.LOWEST, TargetingType = TargetingType.ENEMY
                    }
                },
                new AbilityStage
                {
                    AbilityStageCard = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.COLD, MaxTargets = 1, Value = 5, CastTime = 5, Priority = 0
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        StatType = StatType.INITIATIVE, TargetingPreference = TargetingPreference.LOWEST, TargetingType = TargetingType.FRIENDLY
                    }
                },
                new AbilityStage
                {
                    AbilityStageCard = new AbilityStageCard
                    {
                        AbilityEffectType = AbilityEffectType.RETALIATION, AffinityType = AffinityType.FIRE, MaxTargets = 1, Value = 15, CastTime = 0, Priority = 1
                    },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent
                    {
                        StatType = StatType.BASE_HEALTH, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.ENEMY
                    }
                }
            ]
        };

        private readonly AbilityCard _abilityCard = new() { AbilitySlots = 1, Cooldown = 2 };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _statComponentFactoryMock = new Mock<IStatComponentFactory>();
            _abilityStatsFactory = new AbilityStatsFactory(_statComponentFactoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _statComponentFactoryMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _statComponentFactoryMock.Verify();
            _statComponentFactoryMock.VerifyNoOtherCalls();
        }

        private void SetupCreate()
        {
            for (byte i = 0; i < _linkedStats.Length; i++)
            {
                (StatType statType, uint linkedStatValue) linkedStat = _linkedStats[i];

                StatComponent statComponent = new() { StatID = i, Value = linkedStat.linkedStatValue };
                _statComponentFactoryMock.Setup(library => library.Create(linkedStat.statType, It.IsAny<uint>())).Returns(statComponent).Verifiable();
            }
        }

        private void AssertStats(StatComponent[] statComponents)
        {
            Assert.That(statComponents, Is.Not.Null);
            Assert.That(statComponents, Has.Length.EqualTo(_linkedStats.Length));
            
            StatsComponent statsComponent = new() { StatComponents = statComponents };
            for (byte i = 0; i < _linkedStats.Length; i++)
            {
                (StatType statType, uint linkedStatValue) linkedStat = _linkedStats[i];
                
                Assert.That(statsComponent.GetStat(i), Is.EqualTo(linkedStat.linkedStatValue));
            }
        }

        [Test]
        public void Positive_CreateStats_ConvertsArguments_IntoStatComponents()
        {
            SetupCreate();
            
            StatComponent[] statComponents = _abilityStatsFactory.CreateStats(_abilityStagesComponent, _abilityCard);
            
            AssertStats(statComponents);
        }

        [Test]
        public void Negative_CreateStats_AbilityEffectType_OutOfRange_Throws()
        {
            AbilityStagesComponent abilityStagesComponent = new()
            {
                AbilityStages =
                [
                    new AbilityStage
                    {
                        AbilityStageCard = new AbilityStageCard
                        {
                            AbilityEffectType = (AbilityEffectType) byte.MaxValue, AffinityType = AffinityType.STRIKE, MaxTargets = 1, Value = 10, CastTime = 1, Priority = 0
                        },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent
                        {
                            StatType = StatType.COOLDOWN, TargetingPreference = TargetingPreference.LOWEST, TargetingType = TargetingType.ENEMY
                        }
                    }
                ]
            };
            
            Assert.Throws<ArgumentOutOfRangeException>(() => _abilityStatsFactory.CreateStats(abilityStagesComponent, _abilityCard));
        }

        [Test]
        public void Negative_CreateStats_AbilityEffectType_Overflows_Throws()
        {
            AbilityStageCard abilityStageCard = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, MaxTargets = 1, Value = uint.MaxValue, CastTime = 1, Priority = 0
            };
            
            AbilityStage abilityStage = new()
            {
                AbilityStageCard = abilityStageCard,
                TargetingPreferenceComponent = new TargetingPreferenceComponent
                {
                    StatType = StatType.COOLDOWN, TargetingPreference = TargetingPreference.LOWEST, TargetingType = TargetingType.ENEMY
                }
            };
            
            AbilityStagesComponent abilityStagesComponent = new() { AbilityStages = [abilityStage, abilityStage with { AbilityStageCard = abilityStageCard with { Value = 1 }}] };
            
            Assert.Throws<OverflowException>(() => _abilityStatsFactory.CreateStats(abilityStagesComponent, _abilityCard));
        }
    }
}