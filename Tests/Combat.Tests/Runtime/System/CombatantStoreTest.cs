using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantStoreTest
    {
        private CombatantStore _combatantStore;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantFilter> _lowestHealthFilterMock;
        private Mock<ICombatantFilter> _highestAttackFilterMock;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _lowHealthEntity;
        private CombatantEntity _highHealthEntity;
        private CombatantEntity _lowAttackEntity;
        private CombatantEntity _highAttackEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _lowestHealthFilterMock = new Mock<ICombatantFilter>();
            _highestAttackFilterMock = new Mock<ICombatantFilter>();
        }

        [SetUp]
        public void SetUp()
        { 
            _combatantRepositoryMock.Reset();
            _lowestHealthFilterMock.Reset();
            _highestAttackFilterMock.Reset();
            
            _combatantStore = new CombatantStore(_lowestHealthFilterMock.Object, _highestAttackFilterMock.Object, new CollectionAssertion(), new NumberAssertion());
            
            _highHealthEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 6, Health = 10, Speed = 5 }) { IsFriendly = true, CombatantID = 17 };
            _lowHealthEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 7, Health = 5, Speed = 5 }) { IsFriendly = true, CombatantID = 15 };
            _highAttackEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 10, Health = 7, Speed = 5 }) { IsFriendly = true, CombatantID = 12 };
            _lowAttackEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 5, Health = 6, Speed = 5 }) { IsFriendly = true, CombatantID = 27 };
        }

        private static StatCard GetStatCard(CombatantEntity combatantEntity) => combatantEntity.GetComponent<CombatantStatsComponent>().StatCard;

        private void SetupHighestAttackFilter(CombatantEntity expectedEntity, int expectedLength)
        {
            _highestAttackFilterMock.Setup(library => library.GetEntity(It.Is<CombatantEntity[]>(collection => collection.Length == expectedLength))).Returns(expectedEntity).Verifiable();
        }
        
        private void SetupLowestHealthFilter(CombatantEntity expectedEntity, int expectedCount)
        {
            _lowestHealthFilterMock.Setup(library => library.GetEntity(It.Is<CombatantEntity[]>(collection => collection.Length == expectedCount))).Returns(expectedEntity).Verifiable();
        }

        private void VerifyFilters()
        {
            _highestAttackFilterMock.Verify();
            _highestAttackFilterMock.VerifyNoOtherCalls();
            _lowestHealthFilterMock.Verify();
            _lowestHealthFilterMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(CombatantEntity expectedEntity, byte id)
        { 
            _combatantRepositoryMock.Setup(library => library.Get(id)).Returns(expectedEntity).Verifiable();
        }
        
        private void AssertLowestHealthEntity(CombatantEntity combatantEntity)
        { 
            Assert.That(_combatantStore.LowestHealthCombatant.CombatantID, Is.EqualTo(combatantEntity.CombatantID));
        }
        
        private void AssertHighestAttackEntity(CombatantEntity combatantEntity)
        { 
            Assert.That(_combatantStore.HighestAttackCombatant.CombatantID, Is.EqualTo(combatantEntity.CombatantID));
        }

        [Test]
        public void Positive_RegisterInitial_RegistersCorrectCombatants()
        {
            SetupHighestAttackFilter(_highAttackEntity, 4);
            SetupLowestHealthFilter(_lowHealthEntity, 4);
            SetupRepositoryGet(_highAttackEntity, _highAttackEntity.CombatantID);
            SetupRepositoryGet(_lowHealthEntity, _lowHealthEntity.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity]));
            
            AssertLowestHealthEntity(_lowHealthEntity);
            AssertHighestAttackEntity(_highAttackEntity);
            VerifyFilters();
        }

        [Test]
        public void Positive_RegisterInitial_RegistersSingleEntity()
        {
            SetupHighestAttackFilter(_highHealthEntity, 1);
            SetupLowestHealthFilter(_highHealthEntity, 1);
            SetupRepositoryGet(_highHealthEntity, _highHealthEntity.CombatantID);
            SetupRepositoryGet(_highHealthEntity, _highHealthEntity.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity]));
            
            AssertLowestHealthEntity(_highHealthEntity);
            AssertHighestAttackEntity(_highHealthEntity);
            VerifyFilters();
        }

        [Test]
        public void Negative_RegisterInitial_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _combatantStore.RegisterInitial([]));
        }

        [Test]
        public void Negative_RegisterInitial_ZeroHealthFromFilter_Throws()
        { 
            CombatantEntity zeroHealthEntity = new(_repositoryAsserter, new StatCard { Health = 0, Attack = 100, Speed = 4}) { IsFriendly = true, CombatantID = 52 };
            SetupHighestAttackFilter(zeroHealthEntity, 5);
            SetupLowestHealthFilter(zeroHealthEntity, 5);
            SetupRepositoryGet(zeroHealthEntity, zeroHealthEntity.CombatantID);
            
            Assert.Throws<NumberZeroException>(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity, zeroHealthEntity]));
        }

        [Test]
        public void Positive_RegisterCombatantChange_SingleCombatant_RegistersNewCombatant()
        {
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantChange(_lowHealthEntity.CombatantID, GetStatCard(_lowHealthEntity)));

            AssertLowestHealthEntity(_lowHealthEntity);
            AssertHighestAttackEntity(_lowHealthEntity);
        }

        [Test]
        public void Positive_RegisterCombatantChange_MultipleCombatants_RegistersNewCombatants()
        {
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantChange(_highHealthEntity.CombatantID, GetStatCard(_highHealthEntity)));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantChange(_lowHealthEntity.CombatantID, GetStatCard(_lowHealthEntity)));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantChange(_highAttackEntity.CombatantID, GetStatCard(_highAttackEntity)));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantChange(_lowAttackEntity.CombatantID, GetStatCard(_lowAttackEntity)));
            
            AssertLowestHealthEntity(_lowHealthEntity);
            AssertHighestAttackEntity(_highAttackEntity);
        }

        [Test]
        public void Negative_RegisterCombatantChange_ZeroHealth_Throws()
        { 
            Assert.Throws<NumberZeroException>(() => _combatantStore.RegisterCombatantChange(0, new StatCard { Health = 0, Attack = 5, Speed = 5 }));
        }

        [Test]
        public void Positive_RegisterCombatantDeath_RegisterDeath_NoMatchedID()
        {
            SetupHighestAttackFilter(_highAttackEntity, 4);
            SetupLowestHealthFilter(_lowHealthEntity, 4);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity]));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantDeath(_lowAttackEntity.CombatantID, [_highHealthEntity, _lowHealthEntity, _highAttackEntity]));
            
            AssertLowestHealthEntity(_lowHealthEntity);
            AssertHighestAttackEntity(_highAttackEntity);
        }

        [Test]
        public void Positive_RegisterCombatantDeath_IDMatches_HighestAttack_SwitchesEntity()
        {
            SetupHighestAttackFilter(_lowHealthEntity, 4);
            SetupLowestHealthFilter(_lowHealthEntity, 4);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity]));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantDeath(_highAttackEntity.CombatantID, [_highHealthEntity, _lowHealthEntity, _lowAttackEntity]));
            
            AssertLowestHealthEntity(_lowHealthEntity);
            AssertHighestAttackEntity(_lowHealthEntity);
        }
        
        [Test]
        public void Positive_RegisterCombatantDeath_IDMatches_LowestHealth_SwitchesEntity()
        {
            SetupHighestAttackFilter(_highAttackEntity, 4);
            SetupLowestHealthFilter(_lowAttackEntity, 4);
            SetupRepositoryGet(_highAttackEntity, _highAttackEntity.CombatantID);
            SetupRepositoryGet(_lowHealthEntity, _lowHealthEntity.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity]));
            Assert.DoesNotThrow(() => _combatantStore.RegisterCombatantDeath(_lowHealthEntity.CombatantID, [_highHealthEntity, _highAttackEntity, _lowAttackEntity]));
            
            AssertLowestHealthEntity(_lowAttackEntity);
            AssertHighestAttackEntity(_highAttackEntity);
        }
        
        [Test]
        public void Negative_RegisterCombatantDeath_ZeroHealthFromFilter_Throws()
        { 
            CombatantEntity zeroHealthEntity = new(_repositoryAsserter, new StatCard { Health = 0, Attack = 100, Speed = 4}) { IsFriendly = true, CombatantID = 52 };
            SetupHighestAttackFilter(_highAttackEntity,4);
            SetupLowestHealthFilter(_lowHealthEntity, 4);
            SetupRepositoryGet(zeroHealthEntity, zeroHealthEntity.CombatantID);
            SetupRepositoryGet(_highHealthEntity, _highHealthEntity.CombatantID);
            
            Assert.DoesNotThrow(() => _combatantStore.RegisterInitial([_highHealthEntity, _lowHealthEntity, _highAttackEntity, _lowAttackEntity]));
            SetupHighestAttackFilter(zeroHealthEntity, 4);
            SetupLowestHealthFilter(zeroHealthEntity, 4);
            
            Assert.Throws<NumberZeroException>(() => _combatantStore.RegisterCombatantDeath(_lowHealthEntity.CombatantID, [_lowHealthEntity, _highAttackEntity, _lowAttackEntity, zeroHealthEntity]));
        }
    }
}