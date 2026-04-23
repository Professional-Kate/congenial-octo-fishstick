using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
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
        private Combat.Runtime.System.Factory.CombatantEntityFactory _combatService;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private CombatantCard _wolfCard;
        private CombatantStatsComponent _combatantStatsComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _combatService = new Combat.Runtime.System.Factory.CombatantEntityFactory(_combatantRepositoryMock.Object, new CollectionAssertion(), new UniqueAssertion(), new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));

            _combatantStatsComponent = new CombatantStatsComponent { Health = 3, Attack = 5, Speed = 5 };
            _wolfCard = CombatantCardFactory.CreateCombatantCard(CombatantType.WOLF, new StatCard { Health = 3, Attack = 5, Speed = 5 });
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

        private void VerifyRepositoryAdd(CombatantStatsComponent expectedStats, bool isFriendly)
        {
            // :) CombatantEntity -> Get CombatantStatsComponent -> Compare StatCard to provided AND compare if Entity is friend
            _combatantRepositoryMock.Verify(library => library.Add(It.Is<CombatantEntity>(entity => entity.GetComponent<CombatantStatsComponent>() == expectedStats && entity.IsFriendly == isFriendly)));
        }

        private void VerifyRepositoryNextCombatantID(Times times)
        {
            _combatantRepositoryMock.Verify(library => library.NextCombatantID, times);
        }

        private void SetupNextCombatantIDSequence()
        {
            _combatantRepositoryMock.SetupSequence(library => library.NextCombatantID).Returns(0).Returns(1);
        }
        
        [Test]
        public void Positive_SpawnCombatants_SingleCard_CreatesOneCombatant()
        { 
            SetupContains(0);
            
            _combatService.SpawnCombatants([_wolfCard], true);
            
            VerifyRepositoryAdd(_combatantStatsComponent, true);
            VerifyRepositoryNextCombatantID(Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Positive_SpawnCombatants_DuplicateCard_CreatesMultipleCombatant()
        {
            SetupContains(0);
            SetupContains(1);
            SetupNextCombatantIDSequence();
            
            _combatService.SpawnCombatants([_wolfCard, _wolfCard], true);
            
            VerifyRepositoryAdd(_combatantStatsComponent, true);
            VerifyRepositoryAdd(_combatantStatsComponent, true);
            VerifyRepositoryNextCombatantID(Times.Exactly(2));
            VerifyMocks();
        }
        
        [Test]
        public void Positive_SpawnCombatants_MultipleCards_CreatesMultipleCombatant()
        {
            CombatantCard humanCard = CombatantCardFactory.CreateCombatantCard(CombatantType.HUMAN, new StatCard { Health = 10, Attack = 3, Speed = 3 });
            
            SetupContains(0);
            SetupContains(1);
            SetupNextCombatantIDSequence();
            
            _combatService.SpawnCombatants([_wolfCard, humanCard], true);
            
            VerifyRepositoryAdd(_combatantStatsComponent, true);
            VerifyRepositoryAdd(new CombatantStatsComponent { Health = 10, Attack = 3, Speed = 3 }, true);
            VerifyRepositoryNextCombatantID(Times.Exactly(2));
          
            VerifyMocks();
        }
        
        [Test]
        public void Negative_SpawnCombatants_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatService.SpawnCombatants([], true));

            VerifyMocks();
        }

        [Test]
        public void Negative_SpawnCombatants_DuplicateID_Throws()
        { 
            _combatantRepositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _combatService.SpawnCombatants([_wolfCard], true));
            
            VerifyRepositoryNextCombatantID(Times.Exactly(1));
            VerifyMocks();
        }

        [Test]
        public void Negative_SpawnCombatants_EmptySkills_Throws()
        {
            SetupContains(0);
            CombatantCard noSkills = _wolfCard with { SkillCards = [] };
            
            Assert.Throws<EmptyCollectionException>(() => _combatService.SpawnCombatants([noSkills], true));
            
            VerifyRepositoryNextCombatantID(Times.Exactly(1));
            VerifyMocks();
        }
    }
}