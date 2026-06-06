using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Service;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Asset;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilitySlotCalculatorTest
    {
        private AbilitySlotCalculator _abilitySlotCalculator;
        private Mock<IAssetRepository<AbilityType, AbilityEntity>> _abilityRepositoryMock;

        private CombatantAbilityCard _combatantAbilityCard;
        private AbilityEntity _abilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityRepositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            
            _abilitySlotCalculator = new AbilitySlotCalculator(_abilityRepositoryMock.Object);

            _combatantAbilityCard = new CombatantAbilityCard
                { AbilityType = AbilityType.SLASH, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }};

            _abilityEntity = TestAbilityEntityFactory.Create(AbilityType.SLASH, 1);
        }

        [SetUp]
        public void Setup()
        { 
            _abilityRepositoryMock.Reset();
        }

        private void VerifyRepository()
        {
            _abilityRepositoryMock.Verify();
            _abilityRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(AbilityEntity abilityEntity)
        {
            _abilityRepositoryMock.Setup(library => library.Get(_combatantAbilityCard.AbilityType)).Returns(abilityEntity).Verifiable();
        }

        [Test]
        public void Positive_GetAbilitySlots_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityEntity);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_combatantAbilityCard]);
            
            Assert.That(abilitySlots, Is.EqualTo(1));
            VerifyRepository();
        }
        
        [Test]
        public void Positive_GetAbilitySlots_MultipleAbilities_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityEntity);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_combatantAbilityCard, _combatantAbilityCard]);
            
            Assert.That(abilitySlots, Is.EqualTo(2));
            VerifyRepository();
        }

        [Test]
        public void Positive_GetAbilitySlots_EmptyAbilityCards_ReturnsZero()
        {
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([]);
            
            Assert.That(abilitySlots, Is.EqualTo(0));
            VerifyRepository();
        }
    }
}