using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
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

        private StatCard _friendlyStats;
        private CombatantCard _friendlyCard;
        private CombatantEntity _friendlyEntity;
        
        private StatCard _enemyStats;
        private CombatantCard _enemyCard;
        private CombatantEntity _enemyEntity;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyCombatantStoreMock = new Mock<ICombatantStore>();
            _enemyCombatantStoreMock = new Mock<ICombatantStore>();
            _combatantRepositoryMock =  new Mock<ICombatantRepository>();
            
            _enemyTargetFinder = new EnemyTargetFinder(_friendlyCombatantStoreMock.Object, _enemyCombatantStoreMock.Object, _combatantRepositoryMock.Object, new ObjectNullAssertion(), new FoundAssertion());

            _friendlyStats = new StatCard { Health = 25, Attack = 10, Speed = 10 };
            _friendlyCard = CombatantCardFactory.CreateCombatantCard(CombatantType.BEAR, _friendlyStats);
            _friendlyEntity = CombatantEntityFactory.CreateCombatantEntity(1, true, _friendlyCard);
            
            _enemyStats = new StatCard { Health = 15, Attack = 15, Speed = 10 };
            _enemyCard = CombatantCardFactory.CreateCombatantCard(CombatantType.HUMAN, _enemyStats);
            _enemyEntity = CombatantEntityFactory.CreateCombatantEntity(2, false, _enemyCard);
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

        private static void AssertLowestHealthCombatant(Mock<ICombatantStore> combatantStoreMock, Times times)
        { 
            combatantStoreMock.Verify(library => library.LowestHealthCombatant, times);
        }
        
        private static void AssertHighestAttackCombatant(Mock<ICombatantStore> combatantStoreMock, Times times)
        { 
            combatantStoreMock.Verify(library => library.HighestAttackCombatant, times);
        }
        
        private void VerifyCombatStores()
        {
            _friendlyCombatantStoreMock.Verify();
            _friendlyCombatantStoreMock.VerifyNoOtherCalls();
            _enemyCombatantStoreMock.Verify();
            _enemyCombatantStoreMock.VerifyNoOtherCalls();
        }

        private static void VerifyCombatantEntity(CombatantEntity expectedEntity, CombatantEntity actualEntity)
        { 
            Assert.That(actualEntity.CombatantID, Is.EqualTo(expectedEntity.CombatantID));
        }

        [Test]
        public void Positive_FindBestTarget_FriendlyCombatant_UsesEnemyCombatantStore()
        {
            AbilityCard lowHealthCard = new() { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.LOW_HEALTH } };
            CombatantEntity lowHealthAttacker = CombatantEntityFactory.CreateCombatantEntity(1, true, _friendlyCard with { AbilityCards = [lowHealthCard]});
            
            SetupRepositoryGet(_enemyEntity);
            SetupLowestHealthStore(_enemyCombatantStoreMock, new LowestHealthCombatant { CombatantID = _enemyEntity.CombatantID, Health = _enemyStats.Health });
            
            CombatantEntity combatantEntity = _enemyTargetFinder.FindBestTarget(lowHealthAttacker, AbilityType.BASIC_ATTACK);

            AssertLowestHealthCombatant(_enemyCombatantStoreMock, Times.Once());
            AssertLowestHealthCombatant(_friendlyCombatantStoreMock, Times.Never());

            VerifyCombatantEntity(_enemyEntity, combatantEntity);
            VerifyCombatStores();
        }

        [Test]
        public void Positive_FindBestTarget_EnemyCombatant_UsesFriendlyCombatantStore()
        {
            SetupRepositoryGet(_friendlyEntity);
            SetupHighestAttackStore(_friendlyCombatantStoreMock, new HighestAttackCombatant { CombatantID = _friendlyEntity.CombatantID, Attack = _friendlyStats.Attack });
            
            CombatantEntity combatantEntity = _enemyTargetFinder.FindBestTarget(_enemyEntity, AbilityType.BASIC_ATTACK);

            AssertHighestAttackCombatant(_friendlyCombatantStoreMock, Times.Once());
            AssertHighestAttackCombatant(_enemyCombatantStoreMock, Times.Never());

            VerifyCombatantEntity(_friendlyEntity, combatantEntity);
            VerifyCombatStores();
        }

        [Test]
        public void Negative_FindBestTarget_StoresReturnNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _enemyTargetFinder.FindBestTarget(_friendlyEntity, AbilityType.BASIC_ATTACK));
            Assert.Throws<ArgumentNullException>(() => _enemyTargetFinder.FindBestTarget(_enemyEntity, AbilityType.BASIC_ATTACK));
            
            AssertLowestHealthCombatant(_enemyCombatantStoreMock, Times.Never());
            AssertHighestAttackCombatant(_enemyCombatantStoreMock, Times.Once());
            AssertLowestHealthCombatant(_friendlyCombatantStoreMock, Times.Never());
            AssertHighestAttackCombatant(_friendlyCombatantStoreMock, Times.Once());
            VerifyCombatStores();
        }
        
        [Test]
        public void Negative_FindBestTarget_TargetingType_OutOfRange_Throws()
        {
            AbilityCard badAbilityCard = new() { AbilityType = AbilityType.BASIC_ATTACK,  StrategyCard = new StrategyCard { TargetingType = (TargetingType) 15 }};
            
            CombatantCard badTargetingTypeCard = CombatantCardFactory.CreateCombatantCard(CombatantType.BEAR, _friendlyStats);
            CombatantEntity badEntity = CombatantEntityFactory.CreateCombatantEntity(3, true, badTargetingTypeCard with { AbilityCards = [badAbilityCard]});
            
            Assert.Throws<ArgumentOutOfRangeException>(() => _enemyTargetFinder.FindBestTarget(badEntity, AbilityType.BASIC_ATTACK));
        }
    }
}