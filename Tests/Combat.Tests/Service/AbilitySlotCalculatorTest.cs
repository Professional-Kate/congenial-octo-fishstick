using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Service;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Incremental;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilitySlotCalculatorTest
    {
        private AbilitySlotCalculator _abilitySlotCalculator;
        private Mock<IIncrementalRepository<AbilityDefinition>> _abilityEntityRepositoryMock;

        private EquippedAbility _equippedAbility;
        private readonly AbilityDefinition _abilityDefinition = TestAbilityDefinitionFactory.Create();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEntityRepositoryMock = new Mock<IIncrementalRepository<AbilityDefinition>>();
            
            _abilitySlotCalculator = new AbilitySlotCalculator(_abilityEntityRepositoryMock.Object);

            _equippedAbility = new EquippedAbility
                { AbilityID = 1, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.SELF, Priority = 1 }]};

        }

        [SetUp]
        public void Setup()
        { 
            _abilityEntityRepositoryMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _abilityEntityRepositoryMock.Verify();
            _abilityEntityRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(AbilityDefinition abilityDefinition)
        {
            _abilityEntityRepositoryMock.Setup(library => library.Get(_equippedAbility.AbilityID)).Returns(abilityDefinition).Verifiable();
        }

        [Test]
        public void Positive_GetAbilitySlots_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityDefinition);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_equippedAbility]);
            
            Assert.That(abilitySlots, Is.EqualTo(1));
        }
        
        [Test]
        public void Positive_GetAbilitySlots_MultipleAbilities_ReturnsCorrectSlots()
        {
            SetupRepositoryGet(_abilityDefinition);
            
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([_equippedAbility, _equippedAbility]);
            
            Assert.That(abilitySlots, Is.EqualTo(2));
        }

        [Test]
        public void Positive_GetAbilitySlots_EmptyAbilityCards_ReturnsNothing()
        {
            byte abilitySlots = _abilitySlotCalculator.GetAbilitySlots([]);
            
            Assert.That(abilitySlots, Is.Zero);
        }
    }
}