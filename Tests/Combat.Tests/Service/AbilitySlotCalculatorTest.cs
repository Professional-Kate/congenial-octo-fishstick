using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Service;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Incremental;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilitySlotCalculatorTest
    {
        private AbilitySlotCalculator _abilitySlotCalculator;
        private Mock<IIncrementalRepository<AbilityEntity>> _abilityEntityRepositoryMock;

        private CombatantAbilityCard _combatantAbilityCard;
        private AbilityEntity _abilityEntity;

        private CombatantAbilityEntity _existingEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEntityRepositoryMock = new Mock<IIncrementalRepository<AbilityEntity>>();
            
            _abilitySlotCalculator = new AbilitySlotCalculator(_abilityEntityRepositoryMock.Object);

            _combatantAbilityCard = new CombatantAbilityCard
                { AbilityID = 1, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.SELF, Priority = 1 }]};

        }

        [SetUp]
        public void Setup()
        { 
            _abilityEntity = TestAbilityEntityFactory.Create();
            _existingEntity = TestCombatantAbilityEntityFactory.Create(1, 1);
            
            _abilityEntityRepositoryMock.Reset();
        }

        private void VerifyRepository()
        {
            _abilityEntityRepositoryMock.Verify();
            _abilityEntityRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(AbilityEntity abilityEntity)
        {
            _abilityEntityRepositoryMock.Setup(library => library.Get(_combatantAbilityCard.AbilityID)).Returns(abilityEntity).Verifiable();
        }

        [Test]
        public void Positive_GetAbilitySlots_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityEntity);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_combatantAbilityCard], []);
            
            Assert.That(abilitySlots, Is.EqualTo(1));
            VerifyRepository();
        }
        
        [Test]
        public void Positive_GetAbilitySlots_MultipleAbilities_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityEntity);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_combatantAbilityCard, _combatantAbilityCard], [_existingEntity]);
            
            Assert.That(abilitySlots, Is.EqualTo(3));
            VerifyRepository();
        }

        [Test]
        public void Positive_GetAbilitySlots_EmptyAbilityCards_ReturnsOnlyExisting()
        {
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([], [_existingEntity, _existingEntity]);
            
            Assert.That(abilitySlots, Is.EqualTo(2));
            VerifyRepository();
        }
    }
}