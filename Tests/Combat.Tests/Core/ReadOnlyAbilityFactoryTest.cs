using System.Collections.Immutable;
using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Logging;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Core
{
    [TestFixture]
    public sealed class ReadOnlyAbilityFactoryTest
    {
        private ReadOnlyAbilityFactory _readOnlyAbilityFactory;

        private readonly AbilityEntity _abilityEntity = TestAbilityEntityFactory.Create(1, 1);

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _readOnlyAbilityFactory = new ReadOnlyAbilityFactory();
        }

        private static void AssertReadOnlyAbilities(ReadOnlyAbility[] readOnlyAbilities, AbilityEntity[] abilityEntities)
        {
            for (int i = 0; i < abilityEntities.Length; i++)
            {
                AbilityEntity abilityEntity = abilityEntities[i];
                ReadOnlyAbility readOnlyAbility = readOnlyAbilities[i];
                
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(readOnlyAbility.InstanceID, Is.EqualTo(abilityEntity.InstanceID));
                    Assert.That(readOnlyAbility.AbilityID, Is.EqualTo(abilityEntity.AbilityID));
                    Assert.That(readOnlyAbility.AbilityCard.AbilitySlots, Is.EqualTo(abilityEntity.GetStat(StatType.ABILITY_SLOTS)));
                    Assert.That(readOnlyAbility.AbilityCard.Cooldown, Is.EqualTo(abilityEntity.GetStat(StatType.COOLDOWN)));
                }
            }
        }

        private static void AssertAbilityStages(ReadOnlyAbilityStage[] readOnlyAbilities, ImmutableArray<AbilityStage> abilityStages)
        {
            for (int i = 0; i < abilityStages.Length; i++)
            {
                AbilityStage abilityStage = abilityStages[i];
                ReadOnlyAbilityStage readOnlyAbilityStage = readOnlyAbilities[i];
                    
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(readOnlyAbilityStage.AbilityEffectType, Is.EqualTo(abilityStage.AbilityStageCard.AbilityEffectType));
                    Assert.That(readOnlyAbilityStage.AffinityType, Is.EqualTo(abilityStage.AbilityStageCard.AffinityType));
                    Assert.That(readOnlyAbilityStage.CastTime, Is.EqualTo(abilityStage.AbilityStageCard.CastTime));
                    Assert.That(readOnlyAbilityStage.MaxTargets, Is.EqualTo(abilityStage.AbilityStageCard.MaxTargets));
                    Assert.That(readOnlyAbilityStage.Value, Is.EqualTo(abilityStage.AbilityStageCard.Value));
                    Assert.That(readOnlyAbilityStage.ReadOnlyStrategy.TargetingPreference, Is.EqualTo(abilityStage.TargetingPreferenceComponent.TargetingPreference));
                    Assert.That(readOnlyAbilityStage.ReadOnlyStrategy.StatType, Is.EqualTo(abilityStage.TargetingPreferenceComponent.StatType));
                    Assert.That(readOnlyAbilityStage.ReadOnlyStrategy.TargetingType, Is.EqualTo(abilityStage.TargetingPreferenceComponent.TargetingType));
                }
            }
        }

        [Test]
        public void Positive_Create_ConvertsAbilityEntity()
        { 
            ReadOnlyAbility[] readOnlyAbilities = _readOnlyAbilityFactory.Create([_abilityEntity]);

            Assert.That(readOnlyAbilities, Has.Length.EqualTo(1));
            AssertReadOnlyAbilities(readOnlyAbilities, [_abilityEntity]);
            AssertAbilityStages(readOnlyAbilities[0].ReadOnlyAbilityStages, _abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages);
        }

        [Test]
        public void Positive_Create_DoesNotMutateEntities()
        {
            ReadOnlyAbility[] readOnlyAbilities = _readOnlyAbilityFactory.Create([_abilityEntity, _abilityEntity]);

            Assert.That(readOnlyAbilities, Has.Length.EqualTo(2));
            AssertReadOnlyAbilities(readOnlyAbilities, [_abilityEntity, _abilityEntity]);

            AbilityEntity duplicateEntity = TestAbilityEntityFactory.Create(1, 1);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_abilityEntity.GetStat(StatType.COOLDOWN), Is.EqualTo(duplicateEntity.GetStat(StatType.COOLDOWN)));
                Assert.That(_abilityEntity.GetStat(StatType.ABILITY_SLOTS), Is.EqualTo(duplicateEntity.GetStat(StatType.ABILITY_SLOTS)));
            }
        }

        [Test]
        public void Positive_Create_EmptyArray_ReturnsNothing()
        {
            ReadOnlyAbility[] readOnlyAbilities = _readOnlyAbilityFactory.Create([]);
            
            Assert.That(readOnlyAbilities, Has.Length.EqualTo(0));
        }
    }
}