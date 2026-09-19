using IdelPog.Combat.Core.Service;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.Core.Repository.Incremental;
using Moq;

namespace IdelPog.Combat.Tests.Core
{
    [TestFixture]
    public sealed class StatComponentFactoryTest
    {
        private StatComponentFactory _statComponentFactory;
        private Mock<IStatConfigurationGetter> _statConfigurationGetterMock;
        private Mock<IIncrementalRepository<StatCreation>> _statCreationRepositoryMock;

        private readonly StatCreation _statCreation = new()
        {
            InitialValue = 30
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _statConfigurationGetterMock = new Mock<IStatConfigurationGetter>();
            _statCreationRepositoryMock = new Mock<IIncrementalRepository<StatCreation>>();
            
            _statComponentFactory = new StatComponentFactory(_statConfigurationGetterMock.Object, _statCreationRepositoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _statConfigurationGetterMock.Reset();
            _statCreationRepositoryMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _statConfigurationGetterMock.Verify();
            _statConfigurationGetterMock.VerifyNoOtherCalls();
            _statCreationRepositoryMock.Verify();
            _statCreationRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupGetStatID(StatType statType, byte id)
        {
            _statConfigurationGetterMock.Setup(library => library.GetStatID(statType)).Returns(id).Verifiable();
        }

        private void SetupGetStatCreation(StatCreation statCreation, byte id)
        {
            _statCreationRepositoryMock.Setup(library => library.Get(id)).Returns(statCreation).Verifiable();
        }

        private static void AssertStatComponent(StatComponent statComponent, StatCreation statCreation, uint baseStat, byte expectedID)
        {
            uint expectedValue = statCreation.InitialValue + baseStat;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(statComponent.Value, Is.EqualTo(expectedValue));
                Assert.That(statComponent.StatID, Is.EqualTo(expectedID));
            }
        }
        
        [Test]
        public void Positive_Create_ReturnsCreatedComponent()
        {
            SetupGetStatID(StatType.HEALTH, 1);
            SetupGetStatCreation(_statCreation, 1);
            
            StatComponent statComponent = _statComponentFactory.Create(StatType.HEALTH, 10);

            AssertStatComponent(statComponent, _statCreation, 10, 1);
        }

        [Test]
        public void Positive_Create_ReturnsMultiple()
        {
            SetupGetStatID(StatType.COOLDOWN, 1);
            SetupGetStatID(StatType.SPEED, 2);
            SetupGetStatCreation(_statCreation, 1);
            SetupGetStatCreation(_statCreation, 2);
            
            AssertStatComponent(_statComponentFactory.Create(StatType.SPEED, 10), _statCreation, 10, 2);
            AssertStatComponent(_statComponentFactory.Create(StatType.COOLDOWN, 10), _statCreation, 10, 1);
        }

        [Test]
        public void Positive_CalculateStat_ReturnsCorrectStat()
        {
            SetupGetStatID(StatType.BASE_HEALTH, 4);
            SetupGetStatCreation(_statCreation, 4);
            
            Assert.That(_statComponentFactory.CalculateStat(StatType.BASE_HEALTH, 30),  Is.EqualTo(_statCreation.InitialValue + 30));
        }

        [Test]
        public void Positive_CalculateStat_AndCreate_ReturnSameStat()
        {
            SetupGetStatID(StatType.RETALIATION_DAMAGE, 98);
            SetupGetStatCreation(_statCreation, 98);

            StatComponent statComponent = _statComponentFactory.Create(StatType.RETALIATION_DAMAGE, 10);
            uint statValue = _statComponentFactory.CalculateStat(StatType.RETALIATION_DAMAGE, 10);
            
            Assert.That(statComponent.Value, Is.EqualTo(statValue));
        }
    }
}