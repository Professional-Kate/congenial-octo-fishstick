using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class CombatantTargetFinderTest
    {
        private CombatantTargetFinder _combatantTargetFinder;
        private Mock<ICombatantFilters> _combatantFiltersMock;
        private Mock<IAssetRepository<CombatantStatType, IStatProvider>> _statProviderRepositoryMock;
        private Mock<IStatProvider> _statProviderMock;

        private readonly CombatantEntity _highInitiativeEntity = TestCombatantEntityFactory.CreateCombatantEntity(0, true, new AgilityCard { Speed = 10, Initiative = 100 });
        private readonly CombatantEntity _lowInitiativeEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, new AgilityCard { Speed = 7, Initiative = 3 });
        private readonly CombatantEntity _highSpeedEntity = TestCombatantEntityFactory.CreateCombatantEntity(2, true, new AgilityCard { Speed = 1000, Initiative = 53 });
        private readonly CombatantEntity _lowSpeedEntity = TestCombatantEntityFactory.CreateCombatantEntity(3, true, new AgilityCard { Speed = 5, Initiative = 32 });

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            _statProviderRepositoryMock = new Mock<IAssetRepository<CombatantStatType, IStatProvider>>();
            _statProviderMock = new Mock<IStatProvider>();
            
            _combatantTargetFinder = new CombatantTargetFinder(_combatantFiltersMock.Object, _statProviderRepositoryMock.Object, new NumberAssertion(), new CollectionAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _combatantFiltersMock.Reset();
            _statProviderRepositoryMock.Reset();
            _statProviderMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            VerifyMock();
        }

        private void VerifyMock()
        {
            _combatantFiltersMock.Verify();
            _combatantFiltersMock.VerifyNoOtherCalls();
            _statProviderRepositoryMock.Verify();
            _statProviderRepositoryMock.VerifyNoOtherCalls();
            _statProviderMock.Verify();
            _statProviderMock.VerifyNoOtherCalls();
        }

        private void SetupStatProviderRepository(CombatantStatType statType, IStatProvider statProvider)
        {
            _statProviderRepositoryMock.Setup(library => library.Get(statType)).Returns(statProvider).Verifiable();
        }

        private void SetupCombatantFilters(bool isFriendly, params CombatantEntity[] combatantEntities)
        {
            _combatantFiltersMock.Setup(library => library.GetCombatants(isFriendly)).Returns(combatantEntities).Verifiable();
        }

        private static void SetupStatProvider(Mock<IStatProvider> statProviderMock, Func<CombatantEntity, uint> getComponentStat, params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                uint stat = getComponentStat(combatantEntity);
                statProviderMock.Setup(library => library.GetStat(combatantEntity)).Returns(stat).Verifiable();
            }
        }
        
        private static void AssertCombatantEntities(CombatantEntity[] returnedEntities, params CombatantEntity[] expectedEntities)
        { 
            Assert.That(returnedEntities, Has.Length.EqualTo(expectedEntities.Length));
            for (int i = 0; i < expectedEntities.Length; i++)
            {
                Assert.That(returnedEntities[i], Is.EqualTo(expectedEntities[i]));
            }
        }

        [Test]
        public void Positive_SelectPreferredTargets_SingleTarget_Highest_FindsBestTarget()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _lowSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _highSpeedEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Initiative,_lowSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _highSpeedEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.HIGHEST, CombatantStatType.INITIATIVE, true, 1).ToArray();

            AssertCombatantEntities(combatantEntities, _highInitiativeEntity);
        }
        
        [Test]
        public void Positive_SelectPreferredTargets_MultipleTargets_Highest_FindsBestTarget()
        {
            SetupStatProviderRepository(CombatantStatType.SPEED, _statProviderMock.Object);
            SetupCombatantFilters(true, _highSpeedEntity, _lowSpeedEntity, _lowInitiativeEntity, _highInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Speed,_lowSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _highSpeedEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.HIGHEST, CombatantStatType.SPEED, true, 4).ToArray();

            AssertCombatantEntities(combatantEntities, _highSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _lowSpeedEntity);
        }
        
        [Test]
        public void Positive_SelectPreferredTargets_SingleTarget_Lowest_FindsBestTarget()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _highSpeedEntity, _lowSpeedEntity, _lowInitiativeEntity, _highInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Initiative,_lowSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _highSpeedEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.LOWEST, CombatantStatType.INITIATIVE, true, 1).ToArray();

            AssertCombatantEntities(combatantEntities, _lowInitiativeEntity);
        }
        
        [Test]
        public void Positive_SelectPreferredTargets_MultipleTargets_Lowest_FindsBestTarget()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _highSpeedEntity, _lowSpeedEntity, _lowInitiativeEntity, _highInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Initiative,_lowSpeedEntity, _highInitiativeEntity, _lowInitiativeEntity, _highSpeedEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.LOWEST, CombatantStatType.INITIATIVE, true, 4).ToArray();

            AssertCombatantEntities(combatantEntities, _lowInitiativeEntity, _lowSpeedEntity, _highSpeedEntity, _highInitiativeEntity);
        }

        [Test]
        public void Positive_SelectPreferredTargets_MultipleTargets_NotEnoughCombatants_ReturnsLightArray()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _lowInitiativeEntity, _highInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Initiative, _lowInitiativeEntity, _highInitiativeEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.HIGHEST, CombatantStatType.INITIATIVE, true, 5).ToArray();

            AssertCombatantEntities(combatantEntities, _highInitiativeEntity, _lowInitiativeEntity);
        }

        [Test]
        public void Positive_SelectPreferredTargets_SingleTarget_Lowest_ExtremeValue()
        {
            CombatantEntity maxHealthEntity = TestCombatantEntityFactory.CreateCombatantEntity(5, true, new AgilityCard { Speed = 100, Initiative = uint.MaxValue });
            
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _lowInitiativeEntity, maxHealthEntity, _highInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<AgilityComponent>().Initiative, _lowInitiativeEntity, maxHealthEntity, _highInitiativeEntity);
                
            CombatantEntity[] combatantEntities = _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.LOWEST, CombatantStatType.INITIATIVE, true, 2).ToArray();

            AssertCombatantEntities(combatantEntities, _lowInitiativeEntity, _highInitiativeEntity);
        }

        [Test]
        public void Negative_SelectPreferredTargets_ZeroTargetCount_Throws()
        {
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.LOWEST, CombatantStatType.SPEED, true, 0));
            Assert.That(exception.Source, Is.EqualTo("targetCount"));
        }

        [Test]
        public void Negative_SelectPreferredTargets_FilterReturnsNothing_Throws()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(false);
            
            Assert.Throws<EmptyCollectionException>(() => _combatantTargetFinder.SelectPreferredTargets(TargetingPreference.LOWEST, CombatantStatType.INITIATIVE, false, 1));
        }

        [Test]
        public void Negative_SelectPreferredTargets_UnknownTargetingPreference_Throws()
        {
            SetupStatProviderRepository(CombatantStatType.INITIATIVE, _statProviderMock.Object);
            SetupCombatantFilters(true, _lowInitiativeEntity);
            SetupStatProvider(_statProviderMock, component => component.GetComponent<StatsComponent>().Health, _lowInitiativeEntity);
                
            Assert.Throws<ArgumentOutOfRangeException>(() => _combatantTargetFinder.SelectPreferredTargets((TargetingPreference) 10, CombatantStatType.INITIATIVE, true, 2));
        }
    }
}