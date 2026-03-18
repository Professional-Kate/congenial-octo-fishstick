using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantRepositoryTest
    {
        private CombatantRepository _combatantRepository;
        private Mock<IAssetRepository<byte, CombatantEntity>> _repositoryMock;

        private StatCard _wolfCard; 
        private CombatantEntity _wolfEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<byte, CombatantEntity>>();

            _wolfCard = new StatCard { Health = 3, Attack = 5, Speed = 5 };
            _wolfEntity = new CombatantEntity(new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()), _wolfCard) { IsFriendly = false };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _combatantRepository = new CombatantRepository(_repositoryMock.Object);
        }

        private void VerifyAdd(CombatantEntity combatantEntity, byte id)
        {
            _repositoryMock.Verify(library => library.Add(id,  combatantEntity), Times.Once);
        }
        
        private void VerifyRemove(byte id)
        {
            _repositoryMock.Verify(library => library.Remove(id), Times.Once);
        }

        private void SetupContains(byte id, bool contains)
        { 
            _repositoryMock.Setup(library => library.Contains(id)).Returns(contains).Verifiable();
        }

        private void VerifyRepository()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Positive_Add_AddsNewEntity()
        { 
            _combatantRepository.Add(_wolfEntity);

            VerifyAdd(_wolfEntity, 0);
            VerifyRepository();
        }

        [Test]
        public void Positive_Add_AddMultiple_IncrementsID()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            
            VerifyAdd(_wolfEntity, 0);
            VerifyAdd(_wolfEntity, 1);
            VerifyAdd(_wolfEntity, 2);
            VerifyRepository();
        }

        [Test]
        public void Positive_Clear_RemovesOne()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Clear();

            VerifyRemove(0);
        }

        [Test]
        public void Positive_Clear_RemovesAll()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);

            _combatantRepository.Clear();
            
            VerifyRemove(0);
            VerifyRemove(1);
            VerifyRemove(2);
        }

        [Test]
        public void Positive_Add_ClearAfterAdd_ResetsID()
        { 
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Clear();
            _combatantRepository.Add(_wolfEntity);
            
            VerifyRemove(0);
            _repositoryMock.Verify(library => library.Add(0,  _wolfEntity), Times.Exactly(2));
            _repositoryMock.Verify(library => library.Add(1,  _wolfEntity), Times.Never);
            VerifyRepository();
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            SetupContains(0, true);
            _combatantRepository.Add(_wolfEntity);

            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.True);
            VerifyAdd(_wolfEntity, 0);
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Contains_ReturnsFalse()
        {
            SetupContains(0, false);

            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.False);
            VerifyRepository();
        }
    }
}