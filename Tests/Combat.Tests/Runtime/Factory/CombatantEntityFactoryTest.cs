using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantEntityFactoryTest
    {
        private CombatantEntityFactory _combatService;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private CombatantCreation _wolfCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _combatService = new CombatantEntityFactory(_combatantRepositoryMock.Object, new UniqueAssertion(), new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));

            _wolfCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF, new StatCard { Health = 3, Attack = 5, Speed = 5 });
        }

        [SetUp]
        public void Setup()
        {
            _combatantRepositoryMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupContains(byte id)
        {
            _combatantRepositoryMock.Setup(library => library.Contains(id)).Returns(false).Verifiable();
        }

        private void VerifyRepositoryNextCombatantID()
        {
            _combatantRepositoryMock.Verify(library => library.NextCombatantID, Times.Once);
        }

        [Test]
        public void Positive_SpawnCombatants_SingleCard_CreatesOneCombatant()
        { 
            SetupContains(0);
            
            _combatService.CreateEntity(_wolfCreation);
            
            VerifyRepositoryNextCombatantID();
            VerifyMocks();
        }
        
        [Test]
        public void Negative_SpawnCombatants_DuplicateID_Throws()
        { 
            _combatantRepositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _combatService.CreateEntity(_wolfCreation));
            
            VerifyRepositoryNextCombatantID();
            VerifyMocks();
        }
    }
}