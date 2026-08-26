using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository;
using IdelPog.Combat.Tests.TestFactory;
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
            _wolfStatCard = new StatCard { Health = 3 };
            _wolfCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF, _wolfStatCard);
        }

        [SetUp]
        public void SetUp()
        {
            _combatantRepository = new CombatantRepository(new FoundAssertion());
            _enemyWolfEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, TargetingType.ENEMY, _wolfCreation);
            _friendlyWolfEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, TargetingType.FRIENDLY, _wolfCreation);
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
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            VerifyContains(0, true);
            VerifyContains(1, true);
            VerifyContains(2, true);
        }

        [Test]
        public void Positive_Clear_RemovesOne()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Clear();
            
            VerifyContains(0, false);
        }

        [Test]
        public void Positive_Clear_RemovesAll()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
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
            _combatantRepository.Add(_friendlyWolfEntity);
            
            CombatantEntity[] entities = _combatantRepository.GetAllParticipating().ToArray();
            
            Assert.That(entities, Has.Length.EqualTo(4));
        }

        [Test]
        public void Positive_GetAll_EmptyRepository_ReturnsEmptyArray()
        {
            CombatantEntity[] entities = _combatantRepository.GetAllParticipating().ToArray();
            
            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_GetAll_SomeCombatantsNotParticipating()
        {
            _enemyWolfEntity.RemoveComponent<CombatParticipantComponent>();
            
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_friendlyWolfEntity);
            
            CombatantEntity[] entities = _combatantRepository.GetAllParticipating().ToArray();
            
            Assert.That(entities, Has.Length.EqualTo(1));
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
        public void Positive_HasValidCombatants_ContainsFriendlies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.True);
        }

        [Test]
        public void Positive_HasValidCombatants_ContainsFriendly_ButIsInvalid()
        {
            _friendlyWolfEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            _combatantRepository.Add(_friendlyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.False);
        }

        [Test]
        public void Positive_HasValidCombatants_ContainsEnemy_FriendlyReturnsFalse()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.False);
        }

        [Test]
        public void Positive_HasValidCombatants_SelfRequest_ReturnsEntityMarkedWithSelf()
        {
            _friendlyWolfEntity.ReplaceComponent(new TargetingTypeComponent { TargetingType = TargetingType.SELF });
            _combatantRepository.Add(_friendlyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.SELF);
            
            Assert.That(hasValid, Is.True);
        }

        [Test]
        public void Positive_HasValidCombatants_CombatantsNotParticipating_ReturnsFalse()
        {
            _friendlyWolfEntity.RemoveComponent<CombatParticipantComponent>();
            
            _combatantRepository.Add(_friendlyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.False);
        }
        
        [Test]
        public void Positive_HasValidCombatants_SomeCombatantsNotParticipating_ReturnsFalse()
        {
            _enemyWolfEntity.ReplaceComponent(new TargetingTypeComponent { TargetingType = TargetingType.FRIENDLY });
            _enemyWolfEntity.RemoveComponent<CombatParticipantComponent>();
            
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.True);
        }

        [Test]
        public void Positive_GetCombatants_FriendlyLookingForFriendlies()
        {
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_friendlyWolfEntity);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.FRIENDLY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(1));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
        }

        [Test]
        public void Positive_GetCombatants_FriendlyLookingForEnemies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(1));
            Assert.That(entities, Has.Member(_enemyWolfEntity));
        }

        [Test]
        public void Positive_GetCombatants_EnemyLookingForFriendlies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.FRIENDLY, TargetingType.ENEMY);
            
            Assert.That(entities, Has.Count.EqualTo(0));
        }

        [Test]
        public void Positive_GetCombatants_EnemyLookingForEnemies()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.ENEMY);
            
            Assert.That(entities, Has.Count.EqualTo(2));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
        }
        
        [Test]
        public void Positive_GetCombatants_SomeCombatantsNotParticipating()
        {
            _enemyWolfEntity.RemoveComponent<CombatParticipantComponent>();
            
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(0));
        }

        [Test]
        public void Positive_GetCombatants_SelfTargeting_ReturnsNothing()
        {
            _combatantRepository.Add(_friendlyWolfEntity);
            _combatantRepository.Add(_enemyWolfEntity);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_combatantRepository.GetCombatants(TargetingType.SELF, TargetingType.ENEMY), Has.Count.EqualTo(0));
                Assert.That(_combatantRepository.GetCombatants(TargetingType.SELF, TargetingType.FRIENDLY), Has.Count.EqualTo(0));
            }
        }
    }
}