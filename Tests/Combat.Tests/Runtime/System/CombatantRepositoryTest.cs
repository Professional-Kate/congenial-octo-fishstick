using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantRepositoryTest
    {
        private CombatantRepository _combatantRepository;

        private StatCard _wolfStatCard;
        private CombatantCreation _wolfCreation;
        private CombatantEntity _enemyWolfEntity;
        private CombatantEntity _friendlyWolfEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _wolfStatCard = new StatCard { Health = 3, Attack = 5, Speed = 5 };
            _wolfCreation = CombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF, _wolfStatCard);
        }

        [SetUp]
        public void SetUp()
        {
            _combatantRepository = new CombatantRepository(new FoundAssertion());
            _enemyWolfEntity = CombatantEntityFactory.CreateCombatantEntity(1, false, _wolfCreation);
            _friendlyWolfEntity = CombatantEntityFactory.CreateCombatantEntity(2, true, _wolfCreation);
        }

        private void VerifyContains(byte id, bool contains)
        {
            Assert.That(_combatantRepository.Contains(id), Is.EqualTo(contains));
        }

        [Test]
        public void Positive_Add_AddsNewEntity()
        { 
            _combatantRepository.Add(_enemyWolfEntity);

            VerifyContains(0, true);
        }

        [Test]
        public void Positive_Add_AddMultiple_IncrementsID()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            VerifyContains(0, true);
            VerifyContains(1, true);
            VerifyContains(2, true);
        }

        [Test]
        public void Positive_Clear_RemovesOne()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Clear();
            
            VerifyContains(0, false);
        }

        [Test]
        public void Positive_Clear_RemovesAll()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);

            _combatantRepository.Clear();
            
            VerifyContains(0, false);
            VerifyContains(1, false);
            VerifyContains(2, false);
        }

        [Test]
        public void Positive_Add_ClearAfterAdd_ResetsID()
        { 
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Clear();
            _combatantRepository.Add(_enemyWolfEntity);
            
            VerifyContains(0, true);
            VerifyContains(1, false);
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _combatantRepository.Add(_enemyWolfEntity);

            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.True);
            VerifyContains(0, true);
        }
        
        [Test]
        public void Positive_Contains_ReturnsFalse()
        {
            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Positive_GetAll_ReturnsAll()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity[] entities = _combatantRepository.GetAll().ToArray();
            
            Assert.That(entities, Has.Length.EqualTo(3));
        }

        [Test]
        public void Positive_GetAll_EmptyRepository_ReturnsEmptyArray()
        {
            CombatantEntity[] entities = _combatantRepository.GetAll().ToArray();
            
            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_Get_ReturnsEntity()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity combatantEntity = _combatantRepository.Get(0);
            
            Assert.That(combatantEntity, Is.Not.Null);
            Assert.That(combatantEntity, Is.EqualTo(_enemyWolfEntity));
        }

        [Test]
        public void Negative_Get_KeyNotFound_Throws()
        { 
            Assert.Throws<NotFoundException<byte>>(() => _combatantRepository.Get(0));
        }

        [Test]
        public void Positive_GetFriendlies_ReturnsAllFriendlies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetFriendlies().ToArray();
            
            Assert.That(combatantEntities, Has.Length.EqualTo(1));
        }

        [Test]
        public void Positive_GetFriendlies_EmptyRepository_ReturnsEmptyArray()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetFriendlies().ToArray();
            
            Assert.That(combatantEntities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_GetFriendlies_NoAliveEntities_ReturnsEmptyArray()
        {
            _friendlyWolfEntity.UpdateLifeStatus(false);
            _combatantRepository.Add(_friendlyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetFriendlies().ToArray();
         
            Assert.That(combatantEntities, Has.Length.EqualTo(0));
        }
        
        [Test]
        public void Positive_GetEnemies_ReturnsAllFriendlies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetEnemies().ToArray();
            
            Assert.That(combatantEntities, Has.Length.EqualTo(2));
        }
        
        [Test]
        public void Positive_GetEnemies_EmptyRepository_ReturnsEmptyArray()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetEnemies().ToArray();
            
            Assert.That(combatantEntities, Has.Length.EqualTo(0));
        }
        
        [Test]
        public void Positive_GetEnemies_NoAliveEntities_ReturnsEmptyArray()
        {
            _enemyWolfEntity.UpdateLifeStatus(false);
            _combatantRepository.Add(_enemyWolfEntity);
            
            CombatantEntity[] combatantEntities = _combatantRepository.GetEnemies().ToArray();
         
            Assert.That(combatantEntities, Has.Length.EqualTo(0));
        }
    }
}