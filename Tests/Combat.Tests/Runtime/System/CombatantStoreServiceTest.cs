using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantStoreServiceTest
    {
        private CombatantStoreService _combatantStoreService;
        private Mock<ICombatantStore> _friendlyCombatantStoreMock;
        private Mock<ICombatantStore> _enemyCombatantStoreMock;
        private Mock<ICombatantFilters> _combatantFiltersMock;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _friendlyCombatant;
        private CombatantEntity _enemyCombatant;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyCombatantStoreMock = new Mock<ICombatantStore>();
            _enemyCombatantStoreMock = new Mock<ICombatantStore>();
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(),  new UniqueAssertion());
            
            _combatantStoreService = new CombatantStoreService(_friendlyCombatantStoreMock.Object, _enemyCombatantStoreMock.Object, _combatantFiltersMock.Object, new CollectionAssertion());

            _friendlyCombatant = new CombatantEntity(
                _repositoryAsserter, 
                new CombatantCard { 
                    CombatantType = CombatantType.HUMAN, 
                    TargetingType = TargetingType.HIGH_ATTACK, 
                    IsFriendly = true, 
                    StatCard = new StatCard { Health = 1, Attack = 2, Speed = 3 } }) { CombatantID = 1, IsAlive = true };
            
            _enemyCombatant = new CombatantEntity(
                _repositoryAsserter, 
                new CombatantCard { 
                    CombatantType = CombatantType.BEAR, 
                    TargetingType = TargetingType.LOW_HEALTH, 
                    IsFriendly = false, 
                    StatCard = new StatCard { Health = 3, Attack = 1, Speed = 2 } }) { CombatantID = 2, IsAlive = true };
        }

        [SetUp]
        public void Setup()
        {
            _friendlyCombatantStoreMock.Reset();
            _enemyCombatantStoreMock.Reset();
            _combatantFiltersMock.Reset();
        }

        private void SetupFriendlyFilter(params CombatantEntity[] combatants)
        {
            _combatantFiltersMock.Setup(library => library.GetFriendlies()).Returns(combatants).Verifiable();
        }
        
        private void SetupEnemyFilter(params CombatantEntity[] combatants)
        {
            _combatantFiltersMock.Setup(library => library.GetEnemies()).Returns(combatants).Verifiable();
        }

        private static void VerifyCombatantStoreRegisterInitial(Mock<ICombatantStore> combatantStoreMock, params CombatantEntity[] combatants)
        { 
            combatantStoreMock.Verify(library => library.RegisterInitial(combatants), Times.Once);
        }

        private static void VerifyCombatantChange(Mock<ICombatantStore> combatantStoreMock, CombatantEntity changedCombatant)
        {
            combatantStoreMock.Verify(library => library.RegisterCombatantChange(changedCombatant.CombatantID, changedCombatant.GetComponent<CombatantStatsComponent>().StatCard), Times.Once);
        }

        private static void SetupHighestAttackCombatant(Mock<ICombatantStore> combatantStoreMock, HighestAttackCombatant highestAttackCombatant)
        {
            combatantStoreMock.Setup(library => library.HighestAttackCombatant).Returns(highestAttackCombatant).Verifiable();
        }
        
        private static void SetupLowestHealthCombatant(Mock<ICombatantStore> combatantStoreMock, LowestHealthCombatant lowestHealthCombatant)
        {
            combatantStoreMock.Setup(library => library.LowestHealthCombatant).Returns(lowestHealthCombatant).Verifiable();
        }

        private static void VerifyHighestAttackCombatant(Mock<ICombatantStore> combatantStoreMock)
        {
            combatantStoreMock.Verify(library => library.HighestAttackCombatant, Times.Once);
        }
        
        private static void VerifyLowestHealthCombatant(Mock<ICombatantStore> combatantStoreMock)
        {
            combatantStoreMock.Verify(library => library.LowestHealthCombatant, Times.Once);
        }

        private static void VerifyRegisterCombatantDeath(Mock<ICombatantStore> combatantStoreMock, byte combatantID, params CombatantEntity[] combatants)
        {
            combatantStoreMock.Verify(library => library.RegisterCombatantDeath(combatantID, combatants), Times.Once);
        }

        private void VerifyCombatantStores()
        {
            _friendlyCombatantStoreMock.Verify();
            _friendlyCombatantStoreMock.VerifyNoOtherCalls();
            _enemyCombatantStoreMock.Verify();
            _enemyCombatantStoreMock.VerifyNoOtherCalls();
        }

        private void VerifyFilter()
        {
            _combatantFiltersMock.Verify();
            _combatantFiltersMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_RegisterInitial_RegistersFromFilters()
        {
            SetupFriendlyFilter(_friendlyCombatant);
            SetupEnemyFilter(_enemyCombatant);
            
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterInitial());

            VerifyCombatantStoreRegisterInitial(_friendlyCombatantStoreMock, _friendlyCombatant);
            VerifyCombatantStoreRegisterInitial(_enemyCombatantStoreMock, _enemyCombatant);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Negative_RegisterInitial_FriendlyStore_ReturnsNothing_TThrows()
        {
            SetupFriendlyFilter();
            
            Assert.Throws<EmptyCollectionException>(() => _combatantStoreService.RegisterInitial());
            
            _friendlyCombatantStoreMock.Verify(library => library.RegisterInitial(new []{ _friendlyCombatant }), Times.Never);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Negative_RegisterInitial_EnemyStore_ReturnsNothing_Throws()
        {
            SetupFriendlyFilter(_friendlyCombatant);
            SetupEnemyFilter();
            
            Assert.Throws<EmptyCollectionException>(() => _combatantStoreService.RegisterInitial());
            
            _friendlyCombatantStoreMock.Verify(library => library.RegisterInitial(new []{ _friendlyCombatant }), Times.Once);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Positive_RegisterCombatantChange_FriendlyCombatant_RegistersFriendlyChange()
        {
            StatCard changedCard = new() { Attack = 10, Health = 20, Speed = 15 };
            _friendlyCombatant.UpdateCombatantStats(changedCard);
            
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantChange(_friendlyCombatant));
            
            VerifyCombatantChange(_friendlyCombatantStoreMock, _friendlyCombatant);
            VerifyFilter();
            VerifyCombatantStores();
        }
        
        [Test]
        public void Positive_RegisterCombatantChange_EnemyCombatant_RegistersFriendlyChange()
        {
            StatCard changedCard = new() { Attack = 10, Health = 20, Speed = 15 };
            _enemyCombatant.UpdateCombatantStats(changedCard);
            
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantChange(_enemyCombatant));
            
            VerifyCombatantChange(_enemyCombatantStoreMock, _enemyCombatant);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Positive_RegisterCombatantDeath_FriendlyCombatant_NoIDMatch()
        {
            SetupHighestAttackCombatant(_friendlyCombatantStoreMock, new HighestAttackCombatant { Attack = 10, CombatantID = 10 });
            SetupLowestHealthCombatant(_friendlyCombatantStoreMock, new LowestHealthCombatant { Health = 10, CombatantID = 11 });
                
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantDeath(_friendlyCombatant));

            VerifyHighestAttackCombatant(_friendlyCombatantStoreMock);
            VerifyLowestHealthCombatant(_friendlyCombatantStoreMock);
            VerifyFilter();
            VerifyCombatantStores();
        }
        
        [Test]
        public void Positive_RegisterCombatantDeath_EnemyCombatant_NoIDMatch()
        {
            SetupHighestAttackCombatant(_enemyCombatantStoreMock, new HighestAttackCombatant { Attack = 10, CombatantID = 10 });
            SetupLowestHealthCombatant(_enemyCombatantStoreMock, new LowestHealthCombatant { Health = 10, CombatantID = 11 });
                
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantDeath(_enemyCombatant));

            VerifyHighestAttackCombatant(_enemyCombatantStoreMock);
            VerifyLowestHealthCombatant(_enemyCombatantStoreMock);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Positive_RegisterCombatantDeath_FriendlyCombatant_IDMatch()
        {
            SetupFriendlyFilter(_friendlyCombatant);
            SetupHighestAttackCombatant(_friendlyCombatantStoreMock, new HighestAttackCombatant { Attack = 10, CombatantID = _friendlyCombatant.CombatantID });
            
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantDeath(_friendlyCombatant));
            
            VerifyRegisterCombatantDeath(_friendlyCombatantStoreMock, _friendlyCombatant.CombatantID, _friendlyCombatant);
            VerifyHighestAttackCombatant(_friendlyCombatantStoreMock);
            VerifyFilter();
            VerifyCombatantStores();
        }
        
        [Test]
        public void Positive_RegisterCombatantDeath_EnemyCombatant_IDMatch()
        {
            SetupEnemyFilter(_enemyCombatant);
            SetupLowestHealthCombatant(_enemyCombatantStoreMock, new LowestHealthCombatant { Health = 10, CombatantID = _enemyCombatant.CombatantID });
            
            Assert.DoesNotThrow(() => _combatantStoreService.RegisterCombatantDeath(_enemyCombatant));
            
            VerifyRegisterCombatantDeath(_enemyCombatantStoreMock, _enemyCombatant.CombatantID, _enemyCombatant);
            VerifyLowestHealthCombatant(_enemyCombatantStoreMock);
            VerifyHighestAttackCombatant(_enemyCombatantStoreMock);
            VerifyFilter();
            VerifyCombatantStores();
        }

        [Test]
        public void Negative_RegisterCombatantDeath_FriendlyFilterReturnsNothing_Throws()
        {
            SetupFriendlyFilter();
            SetupHighestAttackCombatant(_friendlyCombatantStoreMock, new HighestAttackCombatant { Attack = 10, CombatantID = _friendlyCombatant.CombatantID });
            
            Assert.Throws<EmptyCollectionException>(() => _combatantStoreService.RegisterCombatantDeath(_friendlyCombatant));
            
            VerifyFilter();
            VerifyCombatantStores();
        }
        
        [Test]
        public void Negative_RegisterCombatantDeath_EnemyFilterReturnsNothing_Throws()
        {
            SetupEnemyFilter();
            SetupLowestHealthCombatant(_enemyCombatantStoreMock, new LowestHealthCombatant { Health = 10, CombatantID = _enemyCombatant.CombatantID });
            
            Assert.Throws<EmptyCollectionException>(() => _combatantStoreService.RegisterCombatantDeath(_enemyCombatant));
            
            VerifyHighestAttackCombatant(_enemyCombatantStoreMock);
            VerifyFilter();
            VerifyCombatantStores();
        }
    }
}