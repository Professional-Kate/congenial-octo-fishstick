using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityStatsFactoryTest
    {
        private AbilityStatsFactory _abilityStatsFactory;

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
            _abilityStatsFactory = new AbilityStatsFactory();
        }

        private static void AssertStats(StatComponent[] statComponents)
        {
            Assert.That(statComponents, Is.Not.Null);
            Assert.That(statComponents, Has.Length.EqualTo(6));
            
            StatsComponent statsComponent = new() { StatComponents = statComponents };
            using (Assert.EnterMultipleScope())
            {
                Assert.That(statsComponent.GetStat(StatType.ABILITY_SLOTS), Is.EqualTo(1));
                Assert.That(statsComponent.GetStat(StatType.COOLDOWN), Is.EqualTo(2));
                Assert.That(statsComponent.GetStat(StatType.CAST_TIME), Is.EqualTo(6));
                Assert.That(statsComponent.GetStat(StatType.ABILITY_DAMAGE), Is.EqualTo(10));
                Assert.That(statsComponent.GetStat(StatType.ABILITY_HEALING), Is.EqualTo(5));
                Assert.That(statsComponent.GetStat(StatType.RETALIATION_DAMAGE), Is.EqualTo(15));
            }
        }

        [Test]
        public void Positive_CreateStats_ConvertsArguments_IntoStatComponents()
        {
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