using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class CombatantAbilityEquipMediatorTest
    {
        private CombatantAbilityEquipMediator _combatantAbilityEquipMediator;
        private Mock<IAbilitySlotCalculator> _abilitySlotCalculatorMock;
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityRepositoryMock;
        private Mock<ICombatantAbilityEntityFactory> _combatantAbilityEntityFactoryMock;
        private Mock<ICombatantAbilityFactory> _combatantAbilityFactoryMock;
        private Mock<IDispatchMany<CombatantAbilityEquipResponse>> _responseDispatcherMock;
        
        private CombatantAbilityCard _combatantAbilityCard;
        private CombatantAbilityEquip _combatantAbilityEquip;
        private CombatantAbility _combatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilitySlotCalculatorMock = new Mock<IAbilitySlotCalculator>();
            _combatantAbilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatantAbilityEntityFactoryMock = new Mock<ICombatantAbilityEntityFactory>();
            _combatantAbilityFactoryMock = new Mock<ICombatantAbilityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantAbilityEquipResponse>>();
            CombatantAbilityAssertion combatantAbilityAssertion = new() { MaxAbilitiesSlots = 1 };
            
            _combatantAbilityEquipMediator = new CombatantAbilityEquipMediator(_abilitySlotCalculatorMock.Object, _combatantAbilityRepositoryMock.Object, _combatantAbilityEntityFactoryMock.Object, _combatantAbilityFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), combatantAbilityAssertion);
            
            _combatantAbilityCard = new CombatantAbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_combatantAbilityCard] };
            _combatantAbility = new CombatantAbility { AbilityType = _combatantAbilityCard.AbilityType, ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 }, Cooldown = 15, PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 10, StrikeDamage = 0, ThrustDamage = 0 }};
        }

        [SetUp]
        public void Setup()
        {
            _combatantAbilityRepositoryMock.Reset();
            _combatantAbilityEntityFactoryMock.Reset();
            _responseDispatcherMock.Reset();
            _combatantAbilityFactoryMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatantAbilityRepositoryMock.Verify();
            _combatantAbilityRepositoryMock.VerifyNoOtherCalls();
            _combatantAbilityEntityFactoryMock.Verify();
            _combatantAbilityEntityFactoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
            _combatantAbilityFactoryMock.Verify();
            _combatantAbilityFactoryMock.VerifyNoOtherCalls();
            _abilitySlotCalculatorMock.Verify();
            _abilitySlotCalculatorMock.VerifyNoOtherCalls();
        }

        private void SetupCalculator(params CombatantAbilityCard[] abilityCards)
        {
            _abilitySlotCalculatorMock.Setup(library => library.GetAbilitySlots(abilityCards)).Returns((byte) abilityCards.Length).Verifiable();
        }

        private void VerifyRepositoryAdd(byte combatantID)
        {
            _combatantAbilityRepositoryMock.Verify(library => library.Add(combatantID, It.IsAny<CombatantAbilityEntity[]>()));
        }

        private void VerifyRepositoryGetAll(byte combatantID)
        {
            _combatantAbilityRepositoryMock.Verify(library => library.GetAll(combatantID), Times.Once);
        }

        private void VerifyFactoryCreate(CombatantAbilityEquip combatantAbilityEquip)
        {
            _combatantAbilityEntityFactoryMock.Verify(library => library.Create(combatantAbilityEquip));
        }

        private void SetupCombatantAbilityFactory(params CombatantAbility[] combatantAbilities)
        {
            _combatantAbilityFactoryMock.Setup(library => library.CreateCombatantAbilities(It.IsAny<IReadOnlyList<CombatantAbilityEntity>>())).Returns(combatantAbilities).Verifiable();
        }

        private void VerifyDispatcherCalled(params CombatantAbilityEquip[] combatantAbilityEquip)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantAbilityEquipResponse[]>(responses => responses.Length == combatantAbilityEquip.Length)));
        }

        [Test]
        public void Positive_HandleMessages_CreatesCombatantAbility_DispatchesResponse()
        {
            SetupCombatantAbilityFactory(_combatantAbility);
            SetupCalculator(_combatantAbilityEquip.AbilityCards);
            
            Assert.DoesNotThrow(() => _combatantAbilityEquipMediator.HandleMessages([_combatantAbilityEquip]));

            VerifyRepositoryGetAll(_combatantAbilityEquip.CombatantID);
            VerifyFactoryCreate(_combatantAbilityEquip);
            VerifyRepositoryAdd(_combatantAbilityEquip.CombatantID);
            VerifyDispatcherCalled(_combatantAbilityEquip);
            VerifyMocks();
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_DispatchesMultipleResponses()
        {
            CombatantAbilityEquip secondEquip = _combatantAbilityEquip with { CombatantID = 2 };
            SetupCombatantAbilityFactory(_combatantAbility, _combatantAbility);
            SetupCalculator(secondEquip.AbilityCards);
            
            Assert.DoesNotThrow(() => _combatantAbilityEquipMediator.HandleMessages([_combatantAbilityEquip, secondEquip]));

            VerifyRepositoryGetAll(_combatantAbilityEquip.CombatantID);
            VerifyRepositoryGetAll(secondEquip.CombatantID);
            VerifyFactoryCreate(_combatantAbilityEquip);
            VerifyFactoryCreate(secondEquip);
            VerifyRepositoryAdd(_combatantAbilityEquip.CombatantID);
            VerifyRepositoryAdd(secondEquip.CombatantID);
            VerifyDispatcherCalled(_combatantAbilityEquip, secondEquip);
            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _combatantAbilityEquipMediator.HandleMessages(null!));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatantAbilityEquipMediator.HandleMessages([]));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_EmptyAbilities_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatantAbilityEquipMediator.HandleMessages([_combatantAbilityEquip with { AbilityCards = [] }]));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_TooManyAbilities_Throws()
        {
            CombatantAbilityEquip doubleEquip = _combatantAbilityEquip with
            {
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard with { AbilityType = AbilityType.STRONG_ATTACK }]
            };
            SetupCalculator(doubleEquip.AbilityCards);
            
            Assert.Throws<TooManyAbilitiesException>(() => _combatantAbilityEquipMediator.HandleMessages([doubleEquip]));
            
            VerifyMocks();
        }
    }
}