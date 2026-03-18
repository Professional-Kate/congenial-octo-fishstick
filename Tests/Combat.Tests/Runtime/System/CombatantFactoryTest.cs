using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantFactoryTest
    {
        private CombatantFactory _combatService;
        private Mock<IAssetRepository<byte, CombatantEntity>> _friendlyRepositoryMock;

        private CombatantCard _wolfCard; 

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyRepositoryMock = new Mock<IAssetRepository<byte, CombatantEntity>>();
            
            _combatService = new CombatantFactory(_friendlyRepositoryMock.Object, new CollectionAssertion(), new UniqueAssertion(), new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));

            _wolfCard = new CombatantCard { CombatantType = CombatantType.WOLF, StatCard = new StatCard { Health = 3, Attack = 5, Speed = 5 }, IsFriendly = true };
        }

        [SetUp]
        public void Setup()
        {
            _friendlyRepositoryMock.Reset();
        }

        private void VerifyRepository()
        {
            _friendlyRepositoryMock.Verify();
            _friendlyRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupContains(byte id)
        {
            _friendlyRepositoryMock.Setup(library => library.Contains(id)).Returns(false).Verifiable();
        }

        private void VerifyRepositoryAdd(byte id, StatCard statCard, bool isFriendly)
        {
            // :) CombatantEntity -> Get CombatantStatsComponent -> Compare StatCard to provided AND compare if Entity is friend
            _friendlyRepositoryMock.Verify(library => library.Add(id, It.Is<CombatantEntity>(entity => entity.GetComponent<CombatantStatsComponent>().StatCard == statCard && entity.IsFriendly == isFriendly)));
        }

        [Test]
        public void Test_SpawnCombatants_SingleCard_CreatesOneCombatant()
        { 
            SetupContains(0);
            
            _combatService.SpawnCombatants([_wolfCard]);
            
            VerifyRepositoryAdd(0, _wolfCard.StatCard, true);
            VerifyRepository();
        }
        
        [Test]
        public void Test_SpawnCombatants_DuplicateCard_CreatesMultipleCombatant()
        {
            SetupContains(0);
            SetupContains(1);
            
            _combatService.SpawnCombatants([_wolfCard, _wolfCard]);
            
            VerifyRepositoryAdd(0, _wolfCard.StatCard, true);
            VerifyRepositoryAdd(1, _wolfCard.StatCard, true);
            VerifyRepository();
        }
        
        [Test]
        public void Test_SpawnCombatants_MultipleCards_CreatesMultipleCombatant()
        {
            SetupContains(0);
            SetupContains(1);
            
            CombatantCard humanCard = new() { CombatantType = CombatantType.HUMAN, StatCard = new StatCard { Health = 10, Attack = 3, Speed = 3 }, IsFriendly = false };
            _combatService.SpawnCombatants([_wolfCard, humanCard]);
            
            VerifyRepositoryAdd(0, _wolfCard.StatCard, true);
            VerifyRepositoryAdd(1, humanCard.StatCard, false);
            VerifyRepository();
        }
        
        [Test]
        public void Negative_SpawnCombatants_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatService.SpawnCombatants([]));

            VerifyRepository();
        }

        [Test]
        public void Negative_SpawnCombatants_DuplicateID_Throws()
        { 
            _friendlyRepositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            
            Assert.Throws<DuplicateEntityException>(() => _combatService.SpawnCombatants([_wolfCard]));
            
            VerifyRepository();
        }
    }
}