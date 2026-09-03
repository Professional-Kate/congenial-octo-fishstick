using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.System.Repository;
using IdelPog.Combat.Tests.TestFactory;

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
            _combatantRepository = new CombatantRepository();
            _enemyWolfEntity = TestCombatantEntityFactory.Create(1, TargetingType.ENEMY, _wolfCreation);
            _friendlyWolfEntity = TestCombatantEntityFactory.Create(2, TargetingType.FRIENDLY, _wolfCreation);
        }

        private void AssertContains(bool contains, params byte[] ids)
        {
            foreach (byte id in ids)
            {
                if (contains)
                {
                    Assert.That(() => _combatantRepository.Get(id), Throws.Nothing);
                }
                else
                {
                    Assert.That(() => _combatantRepository.Get(id), Throws.TypeOf<KeyNotFoundException>());
                }
            }
        }

        [Test]
        public void Positive_Clear_Clears()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            
            _combatantRepository.Clear();
            
            AssertContains(false, _friendlyWolfEntity.CombatantID);
        }

        [Test]
        public void Positive_SeedFriendlyCombatants_AddsCombatants()
        {
            _combatantRepository.SeedFriendlyCombatants([_enemyWolfEntity]);

            AssertContains(true, _enemyWolfEntity.CombatantID);
        }
        
        
        [Test]
        public void Positive_SeedEnemyCombatants_AddsCombatants()
        {
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);

            AssertContains(true, _enemyWolfEntity.CombatantID);
        }

        [Test]
        public void Positive_GetAll_ReturnsAll()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity, _enemyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity, _friendlyWolfEntity]);
            
            CombatantEntity[] entities = _combatantRepository.Enumerate().ToArray();
            
            Assert.That(entities, Has.Length.EqualTo(4));
            AssertContains(true, _friendlyWolfEntity.CombatantID, _friendlyWolfEntity.CombatantID, _enemyWolfEntity.CombatantID, _enemyWolfEntity.CombatantID);
        }

        [Test]
        public void Positive_GetAll_EmptyRepository_ReturnsEmptyArray()
        {
            CombatantEntity[] entities = _combatantRepository.Enumerate().ToArray();
            
            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_Get_ReturnsEntity()
        {
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            CombatantEntity combatantEntity = _combatantRepository.Get(1);
            
            Assert.That(combatantEntity, Is.Not.Null);
            Assert.That(combatantEntity, Is.EqualTo(_enemyWolfEntity));
        }

        [Test]
        public void Negative_Get_KeyNotFound_Throws()
        { 
            Assert.Throws<KeyNotFoundException>(() => _combatantRepository.Get(0));
        }

        [Test]
        public void Positive_HasValidCombatants_ContainsFriendlies()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.True);
        }

        [Test]
        public void Positive_HasValidCombatants_ContainsFriendly_ButIsInvalid()
        {
            _friendlyWolfEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.False);
        }

        [Test]
        public void Positive_HasValidCombatants_ContainsEnemy_FriendlyReturnsFalse()
        {
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            bool hasValid = _combatantRepository.HasValidCombatants(TargetingType.FRIENDLY);
            
            Assert.That(hasValid, Is.False);
        }

        [Test]
        public void Positive_GetCombatants_FriendlyLookingForFriendlies()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.FRIENDLY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(1));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
        }

        [Test]
        public void Positive_GetCombatants_FriendlyLookingForEnemies()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(1));
            Assert.That(entities, Has.Member(_enemyWolfEntity));
        }

        [Test]
        public void Positive_GetCombatants_EnemyLookingForFriendlies()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.FRIENDLY, TargetingType.ENEMY);
            
            Assert.That(entities, Has.Count.EqualTo(0));
        }

        [Test]
        public void Positive_GetCombatants_EnemyLookingForEnemies()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity, _friendlyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity, _enemyWolfEntity, _enemyWolfEntity]);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.ENEMY);
            
            Assert.That(entities, Has.Count.EqualTo(2));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
            Assert.That(entities, Has.Member(_friendlyWolfEntity));
        }
        
        [Test]
        public void Positive_GetCombatants_SomeCombatantsNotAlive()
        {
            _enemyWolfEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            IReadOnlyList<CombatantEntity> entities = _combatantRepository.GetCombatants(TargetingType.ENEMY, TargetingType.FRIENDLY);
            
            Assert.That(entities, Has.Count.EqualTo(0));
        }

        [Test]
        public void Positive_GetCombatants_SelfTargeting_ReturnsNothing()
        {
            _combatantRepository.SeedFriendlyCombatants([_friendlyWolfEntity]);
            _combatantRepository.SeedEnemyCombatants([_enemyWolfEntity]);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_combatantRepository.GetCombatants(TargetingType.SELF, TargetingType.ENEMY), Has.Count.EqualTo(0));
                Assert.That(_combatantRepository.GetCombatants(TargetingType.SELF, TargetingType.FRIENDLY), Has.Count.EqualTo(0));
            }
        }
    }
}