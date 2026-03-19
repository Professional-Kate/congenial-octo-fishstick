using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantFactoryTest
    {
        private CombatantFactory _combatService;
        private Mock<ICombatantRepository> _friendlyRepositoryMock;

        private CombatantCard _wolfCard; 

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyRepositoryMock = new Mock<ICombatantRepository>();
            
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

        private void VerifyRepositoryAdd(StatCard statCard, bool isFriendly)
        {
            // :) CombatantEntity -> Get CombatantStatsComponent -> Compare StatCard to provided AND compare if Entity is friend
            _friendlyRepositoryMock.Verify(library => library.Add(It.Is<CombatantEntity>(entity => entity.GetComponent<CombatantStatsComponent>().StatCard == statCard && entity.IsFriendly == isFriendly)));
        }

        private void VerifyRepositoryNextCombatantID(Times times)
        {
            _friendlyRepositoryMock.Verify(library => library.NextCombatantID, times);
        }

        [Test]
        public void Positive_SpawnCombatants_SingleCard_CreatesOneCombatant()
        { 
            SetupContains(0);
            
            _combatService.SpawnCombatants([_wolfCard]);
            
            VerifyRepositoryAdd(_wolfCard.StatCard, true);
            VerifyRepositoryNextCombatantID(Times.Once());
            VerifyRepository();
        }
        
        [Test]
        public void Positive_SpawnCombatants_DuplicateCard_CreatesMultipleCombatant()
        {
            SetupContains(0);
            SetupContains(1);
            
            _combatService.SpawnCombatants([_wolfCard, _wolfCard]);
            
            VerifyRepositoryAdd(_wolfCard.StatCard, true);
            VerifyRepositoryAdd(_wolfCard.StatCard, true);
            VerifyRepositoryNextCombatantID(Times.Exactly(2));
            VerifyRepository();
        }
        
        [Test]
        public void Positive_SpawnCombatants_MultipleCards_CreatesMultipleCombatant()
        {
            SetupContains(0);
            SetupContains(1);
            
            CombatantCard humanCard = new() { CombatantType = CombatantType.HUMAN, StatCard = new StatCard { Health = 10, Attack = 3, Speed = 3 }, IsFriendly = false };
            _combatService.SpawnCombatants([_wolfCard, humanCard]);
            
            VerifyRepositoryAdd(_wolfCard.StatCard, true);
            VerifyRepositoryAdd(humanCard.StatCard, false);
            VerifyRepositoryNextCombatantID(Times.Exactly(2));
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