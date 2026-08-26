using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class AbilityEntityFactoryTest
    {
        private AbilityEntityFactory _abilityEntityFactory;
        private Mock<IPrioritySorter> _prioritySorterMock;
        
        private AbilityCreation _basicAttackCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _prioritySorterMock = new Mock<IPrioritySorter>();
            
            _abilityEntityFactory = new AbilityEntityFactory(_prioritySorterMock.Object);
            _basicAttackCreation = TestAbilityCreationFactory.Create();
        }

        [SetUp]
        public void Setup()
        {
            _prioritySorterMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _prioritySorterMock.Verify();
            _prioritySorterMock.VerifyNoOtherCalls();
        }

        private void SetupPrioritySorter(IReadOnlyList<AbilityStageCard> abilityStageCards, params AbilityStageCard[] sortedCards)
        {
            _prioritySorterMock.Setup(library => library.Sort(abilityStageCards, It.IsAny<Func<AbilityStageCard, byte>>())).Returns(sortedCards).Verifiable();
        }

        private static void AssertAbilityEntity(AbilityEntity abilityEntity, AbilityCreation abilityCreation)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity, Is.Not.Null);
                Assert.That(abilityEntity.GetComponent<CooldownComponent>().Cooldown, Is.EqualTo(abilityCreation.AbilityCard.Cooldown));
                Assert.That(abilityEntity.AbilitySlots, Is.EqualTo(abilityCreation.AbilityCard.AbilitySlots));
            }
        }

        private static void AssertAbilityStages(AbilityEntity abilityEntity, AbilityStageCard[] abilityStageCards)
        {
            for (int i = 0; i < abilityEntity.AbilityStages.Length; i++)
            {
                AbilityStage abilityStage = abilityEntity.AbilityStages[i];
                AbilityStageCard card = abilityStageCards[i];
                
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(abilityStage.CastTime, Is.EqualTo(card.CastTime));
                    Assert.That(abilityStage.Priority, Is.EqualTo(card.Priority));
                    Assert.That(abilityStage.AbilityEffectType, Is.EqualTo(card.AbilityEffectType));
                    Assert.That(abilityStage.AffinityType, Is.EqualTo(card.AffinityType));
                    Assert.That(abilityStage.MaxTargets, Is.EqualTo(card.MaxTargets));
                    Assert.That(abilityStage.Value, Is.EqualTo(card.Value));
                }
            }
        }

        [Test]
        public void Positive_CreateAbilityEntity_SingleStage_ConvertsAbilityCreation()
        { 
            SetupPrioritySorter(_basicAttackCreation.AbilityStageCards, _basicAttackCreation.AbilityStageCards);
            
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(_basicAttackCreation);
            
            AssertAbilityEntity(abilityEntity, _basicAttackCreation);
            AssertAbilityStages(abilityEntity, _basicAttackCreation.AbilityStageCards);
        }
        
        [Test]
        public void Positive_CreateAbilityEntity_MultipleStages_ConvertsAbilityCreation()
        { 
            AbilityCreation multipleStageCreation = _basicAttackCreation with 
            {
                AbilityStageCards = 
                [
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, CastTime = 0, MaxTargets = 1, Value = 3, Priority = 0 },
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.STRIKE, CastTime = 10, MaxTargets = 1, Value = 7, Priority = 1 },
                    new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.STAB, CastTime = 5, MaxTargets = 2, Value = 10, Priority = 2 }
                ]
            };
            
            SetupPrioritySorter(multipleStageCreation.AbilityStageCards, multipleStageCreation.AbilityStageCards);
            
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(multipleStageCreation);
            
            AssertAbilityEntity(abilityEntity, multipleStageCreation);
            AssertAbilityStages(abilityEntity, multipleStageCreation.AbilityStageCards);
        }
    }
}