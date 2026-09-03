using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Assertion;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Combatant.Mediator;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class AbilityEquipMediatorTest
    {
        private AbilityEquipMediator _abilityEquipMediator;
        private Mock<IAbilitySlotCalculator> _abilitySlotCalculatorMock;
        private Mock<IPrioritySorter> _prioritySorterMock;
        private Mock<IIncrementalRepository<AbilityDefinition>> _abilityDefinitionRepositoryMock;
        private Mock<IDictionary<byte, EquippedAbilityDefinition>> _equippedDefinitionRepositoryMock;
        private Mock<IDispatchMany<AbilityEquipResponse>> _responseDispatcherMock;
        
        private EquippedAbility _equippedAbility;
        private AbilityDefinition _abilityDefinition;
        private AbilityEquip _abilityEquip;
        private EquippedAbilityDefinition _equippedAbilityDefinition;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilitySlotCalculatorMock = new Mock<IAbilitySlotCalculator>();
            _prioritySorterMock = new Mock<IPrioritySorter>();
            _abilityDefinitionRepositoryMock = new Mock<IIncrementalRepository<AbilityDefinition>>();
            _equippedDefinitionRepositoryMock = new Mock<IDictionary<byte, EquippedAbilityDefinition>>();
            _responseDispatcherMock = new Mock<IDispatchMany<AbilityEquipResponse>>();
            AbilityAssertion abilityAssertion = new() { MaxAbilitiesSlots = 1 };
            
            _abilityEquipMediator = new AbilityEquipMediator(_abilitySlotCalculatorMock.Object, _prioritySorterMock.Object, _abilityDefinitionRepositoryMock.Object, _equippedDefinitionRepositoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), abilityAssertion, new PriorityAssertion());
            
            _equippedAbility = new EquippedAbility { AbilityID = 0, StrategyCards = [new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0}]};
            _abilityDefinition = TestAbilityDefinitionFactory.Create();
            _abilityEquip = new AbilityEquip { CombatantID = 1, EquippedAbilities = [_equippedAbility] };
            _equippedAbilityDefinition = new EquippedAbilityDefinition { CombatantID = _abilityEquip.CombatantID, EquippedAbilities = [_equippedAbility] };
        }

        [SetUp]
        public void Setup()
        {
            _abilitySlotCalculatorMock.Reset();
            _equippedDefinitionRepositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _equippedDefinitionRepositoryMock.Verify();
            _equippedDefinitionRepositoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
            _abilitySlotCalculatorMock.Verify();
            _abilitySlotCalculatorMock.VerifyNoOtherCalls();
        }

        private void SetupCalculator(params EquippedAbility[] equippedAbilityStages)
        {
            _abilitySlotCalculatorMock.Setup(library => library.GetAbilitySlots(equippedAbilityStages)).Returns((byte)(equippedAbilityStages.Length)).Verifiable();
        }
        
        private void SetupSort(EquippedAbility[] equippedAbilities)
        {
            foreach (EquippedAbility ability in equippedAbilities)
            {
                _prioritySorterMock.Setup(library => library.Sort(ability.StrategyCards, It.IsAny<Func<StrategyCard, byte>>())).Returns([..ability.StrategyCards]).Verifiable();
            }
        }

        private void SetupAbilityDefinitionGet(AbilityDefinition abilityDefinition, byte abilityID)
        {
            _abilityDefinitionRepositoryMock.Setup(library => library.Get(abilityID)).Returns(abilityDefinition).Verifiable();
        }

        private void VerifyRepositoryAddAbility(EquippedAbilityDefinition equippedAbilityDefinition)
        {
            _equippedDefinitionRepositoryMock.Verify(library => library.Add(equippedAbilityDefinition.CombatantID, It.Is<EquippedAbilityDefinition>(definition => definition.CombatantID == equippedAbilityDefinition.CombatantID)), Times.Once);
        }

        private void VerifyDispatcherCalled(params AbilityEquip[] combatantAbilityEquip)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<AbilityEquipResponse[]>(responses => responses.Length == combatantAbilityEquip.Length)));
        }

        private void VerifyGetAbilitySlots(EquippedAbility[] equippedAbilityStages)
        { 
            _abilitySlotCalculatorMock.Verify(library => library.GetAbilitySlots(equippedAbilityStages));
        }

        [Test]
        public void Positive_HandleMessages_CreatesCombatantAbility_DispatchesResponse()
        {
            SetupSort(_abilityEquip.EquippedAbilities);
            SetupCalculator(_abilityEquip.EquippedAbilities);
            SetupAbilityDefinitionGet(_abilityDefinition, _equippedAbility.AbilityID);
                
            Assert.DoesNotThrow(() => _abilityEquipMediator.HandleMessages([_abilityEquip]));

            VerifyRepositoryAddAbility(_equippedAbilityDefinition);
            VerifyDispatcherCalled(_abilityEquip);
            VerifyGetAbilitySlots([_equippedAbility]);
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_DispatchesMultipleResponses()
        {
            AbilityEquip secondEquip = _abilityEquip with { CombatantID = 2 };
            SetupSort(_abilityEquip.EquippedAbilities);
            SetupSort(secondEquip.EquippedAbilities);
            SetupCalculator(secondEquip.EquippedAbilities);
            SetupAbilityDefinitionGet(_abilityDefinition, _equippedAbility.AbilityID);
            
            Assert.DoesNotThrow(() => _abilityEquipMediator.HandleMessages([_abilityEquip, secondEquip]));
            
            VerifyRepositoryAddAbility(_equippedAbilityDefinition);
            VerifyRepositoryAddAbility(_equippedAbilityDefinition with { CombatantID = 2 });
            VerifyDispatcherCalled(_abilityEquip, secondEquip);
        }

        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _abilityEquipMediator.HandleMessages(null!));
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _abilityEquipMediator.HandleMessages([]));
        }

        [Test]
        public void Negative_HandleMessages_EmptyAbilities_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _abilityEquipMediator.HandleMessages([_abilityEquip with { EquippedAbilities = [] }]));
        }

        [Test]
        public void Negative_HandleMessages_TooManyAbilities_Throws()
        {
            AbilityEquip doubleEquip = _abilityEquip with
            {
                EquippedAbilities = [_equippedAbility, _equippedAbility with { AbilityID = 1 }]
            };
            
            SetupCalculator(doubleEquip.EquippedAbilities);
            
            Assert.Throws<TooManyAbilitiesException>(() => _abilityEquipMediator.HandleMessages([doubleEquip]));
        }
    }
}