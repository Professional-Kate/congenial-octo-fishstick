using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantEntityFactoryTest
    {
        private CombatantEntityFactory _combatantEntityFactory;
        private Mock<IStatConfigurationGetter> _statConfigurationGetterMock;
        private Mock<IStatComponentFactory> _statComponentFactoryMock;

        private readonly CombatantDefinition _wolfDefinition = TestCombatantDefinitionFactory.Create(0, CombatantType.WOLF);
        private readonly CombatantDefinition _humanDefinition = TestCombatantDefinitionFactory.Create(1, CombatantType.HUMAN);

        private readonly (StatType StatType, uint LinkedStatValue)[] _linkedStats =
        [
            (StatType.BASE_HEALTH, 10),
            (StatType.HEALTH, 10),
            (StatType.SPEED, 10),
            (StatType.INITIATIVE, 5)
        ];
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _statConfigurationGetterMock = new Mock<IStatConfigurationGetter>();
            _statComponentFactoryMock = new Mock<IStatComponentFactory>();
        }

        [SetUp]
        public void Setup()
        {
            _statConfigurationGetterMock.Reset();
            _statComponentFactoryMock.Reset();
            
            // need to reset instance ID
            _combatantEntityFactory = new CombatantEntityFactory(_statConfigurationGetterMock.Object, _statComponentFactoryMock.Object);
            
        }

        [TearDown]
        public void TearDown()
        {
            _statConfigurationGetterMock.Verify();
            _statConfigurationGetterMock.VerifyNoOtherCalls();
            _statComponentFactoryMock.Verify();
            _statComponentFactoryMock.VerifyNoOtherCalls();
        }
        
        private void SetupGetID()
        {
            for (byte i = 0; i < _linkedStats.Length; i++)
            {
                (StatType StatType, uint _) requestedStatType = _linkedStats[i];
                _statConfigurationGetterMock.Setup(library => library.GetStatID(requestedStatType.StatType)).Returns(i).Verifiable();
            }
        }

        private void SetupCreateStatComponent()
        {
            for (byte i = 0; i < _linkedStats.Length; i++)
            { 
                (StatType StatType, uint LinkedStatValue) requestedStatType = _linkedStats[i];
                
                StatComponent statComponent = new() { StatID = i, Value =  requestedStatType.LinkedStatValue };
                _statComponentFactoryMock.Setup(library => library.Create(requestedStatType.StatType, requestedStatType.LinkedStatValue)).Returns(statComponent).Verifiable();
            }
        }

        private static void AssertLength(CombatantEntity[] combatantEntities, int expectedLength)
        {
            Assert.That(combatantEntities, Has.Length.EqualTo(expectedLength));
        }
        
        private static void AssertEntity(CombatantEntity combatantEntity, CombatantDefinition combatantDefinition, TargetingType targetingType, byte instanceID)
        {
            Assert.That(combatantEntity, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantEntity.TargetingType, Is.EqualTo(targetingType));
                Assert.That(combatantEntity.CombatantID, Is.EqualTo(combatantDefinition.CombatantID));
                Assert.That(combatantEntity.CombatantType, Is.EqualTo(combatantDefinition.CombatantType));
                Assert.That(combatantEntity.InstanceID, Is.EqualTo(instanceID));
                Assert.That(combatantEntity.GetStat(StatType.BASE_HEALTH), Is.EqualTo(combatantDefinition.HealthCard.BaseHealth));
                Assert.That(combatantEntity.GetStat(StatType.HEALTH), Is.EqualTo(combatantDefinition.HealthCard.Health));
                Assert.That(combatantEntity.GetStat(StatType.INITIATIVE), Is.EqualTo(combatantDefinition.AgilityCard.Initiative));
                Assert.That(combatantEntity.GetStat(StatType.SPEED), Is.EqualTo(combatantDefinition.AgilityCard.Speed));
            }
        }

        [Test]
        public void Positive_Create_ConvertsDefinition_IntoEntity()
        {
            SetupGetID();
            SetupCreateStatComponent();
            
            CombatantEntity[] combatantEntities = _combatantEntityFactory.Create([_humanDefinition], TargetingType.ENEMY);
            
            AssertLength(combatantEntities, 1);
            AssertEntity(combatantEntities[0], _humanDefinition, TargetingType.ENEMY, 0);
        }

        [Test]
        public void Positive_Create_MultipleEntities()
        {
            SetupCreateStatComponent();
            SetupGetID();
            
            CombatantEntity[] combatantEntities = _combatantEntityFactory.Create([_humanDefinition, _wolfDefinition, _wolfDefinition], TargetingType.FRIENDLY);
            
            AssertLength(combatantEntities, 3);
            AssertEntity(combatantEntities[0], _humanDefinition, TargetingType.FRIENDLY, 0);
            AssertEntity(combatantEntities[1], _wolfDefinition, TargetingType.FRIENDLY, 1);
            AssertEntity(combatantEntities[2], _wolfDefinition, TargetingType.FRIENDLY, 2);
        }

        [Test]
        public void Negative_Create_Overflows_InstanceID_Throws()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                _combatantEntityFactory.Create([_wolfDefinition], TargetingType.ENEMY);
            }
            
            Assert.Throws<OverflowException>(() => _combatantEntityFactory.Create([_wolfDefinition], TargetingType.ENEMY));
            _statComponentFactoryMock.Verify(library => library.Create(It.IsAny<StatType>(), It.IsAny<uint>()));
        }
    }
}