using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EnemyTargetFinderTest
    {
        private EnemyTargetFinder _enemyTargetFinder;
        private Mock<ICombatantStore> _friendlyCombatantStoreMock;
        private Mock<ICombatantStore> _enemyCombatantStoreMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityEntityRepositoryMock;

        private StatCard _friendlyStats;
        private CombatantCreation _friendlyCreation;
        private CombatantEntity _friendlyEntity;
        private CombatantAbilityEntity _friendlyAbilityEntity;
        
        private StatCard _enemyStats;
        private CombatantCreation _enemyCreation;
        private CombatantEntity _enemyEntity;
        private CombatantAbilityEntity _enemyAbilityEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyCombatantStoreMock = new Mock<ICombatantStore>();
            _enemyCombatantStoreMock = new Mock<ICombatantStore>();
            _combatantRepositoryMock =  new Mock<ICombatantRepository>();
            _combatantAbilityEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            
            _enemyTargetFinder = new EnemyTargetFinder(_friendlyCombatantStoreMock.Object, _enemyCombatantStoreMock.Object, _combatantAbilityEntityRepositoryMock.Object, _combatantRepositoryMock.Object, new ObjectNullAssertion(), new FoundAssertion());

            _friendlyStats = new StatCard { Health = 25, Attack = 10, Speed = 10 };
            _friendlyCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.BEAR, _friendlyStats);
            _friendlyEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _friendlyCreation);
            _friendlyAbilityEntity = TestCombatantAbilityEntityFactory.Create(_friendlyEntity.CombatantID, AbilityType.BASIC_ATTACK);
            _friendlyAbilityEntity.AddComponent(new TargetingTypeComponent { TargetingType = TargetingType.LOW_HEALTH });
            
            _enemyStats = new StatCard { Health = 15, Attack = 15, Speed = 10 };
            _enemyCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, _enemyStats);
            _enemyEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, false, _enemyCreation);
            _enemyAbilityEntity = TestCombatantAbilityEntityFactory.Create(_enemyEntity.CombatantID, AbilityType.BASIC_ATTACK);
            _enemyAbilityEntity.AddComponent(new TargetingTypeComponent { TargetingType = TargetingType.HIGH_ATTACK });
        }

        [SetUp]
        public void Setup()
        {
            _friendlyCombatantStoreMock.Reset();
            _enemyCombatantStoreMock.Reset();
            _combatantRepositoryMock.Reset();
        }

        private void SetupRepositoryGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private static void SetupLowestHealthStore(Mock<ICombatantStore> combatantStoreMock, LowestHealthCombatant lowestHealthCombatant)
        { 
            combatantStoreMock.Setup(library => library.LowestHealthCombatant).Returns(lowestHealthCombatant).Verifiable();
        }
        
        private static void SetupHighestAttackStore(Mock<ICombatantStore> combatantStoreMock, HighestAttackCombatant highestAttackCombatant)
        { 
            combatantStoreMock.Setup(library => library.HighestAttackCombatant).Returns(highestAttackCombatant).Verifiable();
        }

        private void SetupAbilityRepositoryGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _combatantAbilityEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private static void AssertLowestHealthCombatant(Mock<ICombatantStore> combatantStoreMock, Times times)
        { 
            combatantStoreMock.Verify(library => library.LowestHealthCombatant, times);
        }
        
        private static void AssertHighestAttackCombatant(Mock<ICombatantStore> combatantStoreMock, Times times)
        { 
            combatantStoreMock.Verify(library => library.HighestAttackCombatant, times);
        }
        
        private void VerifyMocks()
        {
            _friendlyCombatantStoreMock.Verify();
            _friendlyCombatantStoreMock.VerifyNoOtherCalls();
            _enemyCombatantStoreMock.Verify();
            _enemyCombatantStoreMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _combatantAbilityEntityRepositoryMock.Verify();
            _combatantAbilityEntityRepositoryMock.VerifyNoOtherCalls();
        }

        private static void VerifyCombatantEntity(CombatantEntity expectedEntity, CombatantEntity actualEntity)
        { 
            Assert.That(actualEntity.CombatantID, Is.EqualTo(expectedEntity.CombatantID));
        }

        [Test]
        public void Positive_FindBestTarget_FriendlyCombatant_UsesEnemyCombatantStore()
        {
            CombatantEntity lowHealthAttacker = TestCombatantEntityFactory.CreateCombatantEntity(1, true, _friendlyCreation);
                
            SetupRepositoryGet(_enemyEntity);
            SetupLowestHealthStore(_enemyCombatantStoreMock, new LowestHealthCombatant { CombatantID = _enemyEntity.CombatantID, Health = _enemyStats.Health });
            SetupAbilityRepositoryGet(_friendlyAbilityEntity);
            
            CombatantEntity combatantEntity = _enemyTargetFinder.FindBestTarget(lowHealthAttacker, AbilityType.BASIC_ATTACK);

            AssertLowestHealthCombatant(_enemyCombatantStoreMock, Times.Once());
            AssertLowestHealthCombatant(_friendlyCombatantStoreMock, Times.Never());

            VerifyCombatantEntity(_enemyEntity, combatantEntity);
            VerifyMocks();
        }

        [Test]
        public void Positive_FindBestTarget_EnemyCombatant_UsesFriendlyCombatantStore()
        {
            SetupRepositoryGet(_friendlyEntity);
            SetupHighestAttackStore(_friendlyCombatantStoreMock, new HighestAttackCombatant { CombatantID = _friendlyEntity.CombatantID, Attack = _friendlyStats.Attack });
            SetupAbilityRepositoryGet(_enemyAbilityEntity);
            
            CombatantEntity combatantEntity = _enemyTargetFinder.FindBestTarget(_enemyEntity, AbilityType.BASIC_ATTACK);

            AssertHighestAttackCombatant(_friendlyCombatantStoreMock, Times.Once());
            AssertHighestAttackCombatant(_enemyCombatantStoreMock, Times.Never());

            VerifyCombatantEntity(_friendlyEntity, combatantEntity);
            VerifyMocks();
        }

        [Test]
        public void Negative_FindBestTarget_StoresReturnNull_Throws()
        {
            SetupAbilityRepositoryGet(_friendlyAbilityEntity);
            SetupAbilityRepositoryGet(_enemyAbilityEntity);
            
            Assert.Throws<ArgumentNullException>(() => _enemyTargetFinder.FindBestTarget(_friendlyEntity, AbilityType.BASIC_ATTACK));
            Assert.Throws<ArgumentNullException>(() => _enemyTargetFinder.FindBestTarget(_enemyEntity, AbilityType.BASIC_ATTACK));
            
            _combatantRepositoryMock.Verify(library => library.Get(0), Times.Exactly(2));
            AssertLowestHealthCombatant(_enemyCombatantStoreMock, Times.Once());
            AssertHighestAttackCombatant(_enemyCombatantStoreMock, Times.Never());
            AssertLowestHealthCombatant(_friendlyCombatantStoreMock, Times.Never());
            AssertHighestAttackCombatant(_friendlyCombatantStoreMock, Times.Once());
            VerifyMocks();
        }
        
        [Test]
        public void Negative_FindBestTarget_TargetingType_OutOfRange_Throws()
        {
            CombatantAbilityEntity badEntity = TestCombatantAbilityEntityFactory.Create(_enemyEntity.CombatantID, AbilityType.BASIC_ATTACK);
            badEntity.AddComponent(new TargetingTypeComponent { TargetingType = (TargetingType) 12 });
            
            SetupAbilityRepositoryGet(badEntity);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => _enemyTargetFinder.FindBestTarget(_enemyEntity, AbilityType.BASIC_ATTACK));
            
            VerifyMocks();
        }
    }
}