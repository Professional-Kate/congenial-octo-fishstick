using IdelPog.Combat.Assertion;
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
            
            _combatantAbilityCard = new CombatantAbilityCard { AbilityID = 0, StrategyCards = [new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0}]};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_combatantAbilityCard] };
        }

        [SetUp]
        public void Setup()
        {
            _abilitySlotCalculatorMock.Reset();
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

        private void SetupCalculator(CombatantAbilityEntity[] existingEntities, params CombatantAbilityCard[] abilityCards)
        {
            _abilitySlotCalculatorMock.Setup(library => library.GetAbilitySlots(abilityCards, existingEntities)).Returns((byte)(abilityCards.Length + existingEntities.Length)).Verifiable();
        }

        private void VerifyRepositoryAddAbilities(byte combatantID)
        {
            _combatantAbilityRepositoryMock.Verify(library => library.AddAbilities(combatantID, It.IsAny<CombatantAbilityEntity[]>()));
        }

        private void SetupRepositoryGetAll(byte combatantID, params CombatantAbilityEntity[] combatantAbilityEntities)
        {
            _combatantAbilityRepositoryMock.Setup(library => library.GetAll(combatantID)).Returns(combatantAbilityEntities).Verifiable();
        }

        private void VerifyFactoryCreate(CombatantAbilityEquip combatantAbilityEquip)
        {
            _combatantAbilityEntityFactoryMock.Verify(library => library.Create(combatantAbilityEquip));
        }

        private void SetupCombatantAbilityFactory(params byte[] combatantAbilityIDs)
        {
            _combatantAbilityFactoryMock.Setup(library => library.GetCombatantAbilityIDs(It.IsAny<IReadOnlyList<CombatantAbilityEntity>>())).Returns(combatantAbilityIDs).Verifiable();
        }

        private void VerifyDispatcherCalled(params CombatantAbilityEquip[] combatantAbilityEquip)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantAbilityEquipResponse[]>(responses => responses.Length == combatantAbilityEquip.Length)));
        }

        private void VerifyGetAbilitySlots(CombatantAbilityCard[] combatantAbilityCards, CombatantAbilityEntity[] combatantAbilityEntities)
        { 
            _abilitySlotCalculatorMock.Verify(library => library.GetAbilitySlots(combatantAbilityCards, combatantAbilityEntities));
        }

        [Test]
        public void Positive_HandleMessages_CreatesCombatantAbility_DispatchesResponse()
        {
            SetupCombatantAbilityFactory(1);
            SetupCalculator([], _combatantAbilityEquip.AbilityCards);
            SetupRepositoryGetAll(_combatantAbilityEquip.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantAbilityEquipMediator.HandleMessages([_combatantAbilityEquip]));

            VerifyFactoryCreate(_combatantAbilityEquip);
            VerifyRepositoryAddAbilities(_combatantAbilityEquip.CombatantID);
            VerifyDispatcherCalled(_combatantAbilityEquip);
            VerifyGetAbilitySlots([_combatantAbilityCard], []);
            VerifyMocks();
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_DispatchesMultipleResponses()
        {
            CombatantAbilityEquip secondEquip = _combatantAbilityEquip with { CombatantID = 2 };
            SetupCombatantAbilityFactory(1, 2);
            SetupCalculator([], secondEquip.AbilityCards);
            SetupRepositoryGetAll(_combatantAbilityEquip.CombatantID);
            SetupRepositoryGetAll(secondEquip.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantAbilityEquipMediator.HandleMessages([_combatantAbilityEquip, secondEquip]));
            
            VerifyFactoryCreate(_combatantAbilityEquip);
            VerifyFactoryCreate(secondEquip);
            VerifyRepositoryAddAbilities(_combatantAbilityEquip.CombatantID);
            VerifyRepositoryAddAbilities(secondEquip.CombatantID);
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
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard with { AbilityID = 1 }]
            };
            
            SetupRepositoryGetAll(_combatantAbilityEquip.CombatantID);
            SetupCalculator([], doubleEquip.AbilityCards);
            
            Assert.Throws<TooManyAbilitiesException>(() => _combatantAbilityEquipMediator.HandleMessages([doubleEquip]));
            
            VerifyMocks();
        }
    }
}