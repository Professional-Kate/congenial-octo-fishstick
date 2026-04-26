using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
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
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityRepositoryMock;
        private Mock<ICombatantAbilityEntityFactory> _combatantAbilityEntityFactoryMock;
        private Mock<ICombatantAbilityFactory> _combatantAbilityFactoryMock;
        private Mock<IDispatchMany<CombatantAbilityEquipResponse>> _responseDispatcherMock;
        
        private AbilityCard _abilityCard;
        private CombatantAbilityEquip _combatantAbilityEquip;
        private CombatantAbility _combatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatantAbilityEntityFactoryMock = new Mock<ICombatantAbilityEntityFactory>();
            _combatantAbilityFactoryMock = new Mock<ICombatantAbilityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantAbilityEquipResponse>>();
            
            _combatantAbilityEquipMediator = new CombatantAbilityEquipMediator(_combatantAbilityRepositoryMock.Object, _combatantAbilityEntityFactoryMock.Object, _combatantAbilityFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
            
            _abilityCard = new AbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_abilityCard] };
            _combatantAbility = new CombatantAbility { AbilityType = _abilityCard.AbilityType, Damage = 10, Cooldown = 15 };
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
    }
}