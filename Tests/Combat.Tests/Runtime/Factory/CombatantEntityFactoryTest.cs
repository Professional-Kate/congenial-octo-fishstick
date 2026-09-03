using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantEntityFactoryTest
    {
        private CombatantEntityFactory _combatantEntityFactory;

        private readonly CombatantDefinition _wolfDefinition = TestCombatantDefinitionFactory.Create(0, CombatantType.WOLF);
        private readonly CombatantDefinition _humanDefinition = TestCombatantDefinitionFactory.Create(1, CombatantType.HUMAN);

        [SetUp]
        public void Setup()
        {
            _combatantEntityFactory = new CombatantEntityFactory();
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
                Assert.That(combatantEntity.GetComponent<HealthComponent>().Health, Is.EqualTo(combatantDefinition.StatCard.Health));
                Assert.That(combatantEntity.GetComponent<AgilityComponent>().Initiative, Is.EqualTo(combatantDefinition.AgilityCard.Initiative));
                Assert.That(combatantEntity.GetComponent<AgilityComponent>().Speed, Is.EqualTo(combatantDefinition.AgilityCard.Speed));
            }
        }

        [Test]
        public void Positive_Create_ConvertsDefinition_IntoEntity()
        {
            CombatantEntity[] combatantEntities = _combatantEntityFactory.Create([_humanDefinition], TargetingType.ENEMY);
            
            AssertLength(combatantEntities, 1);
            AssertEntity(combatantEntities[0], _humanDefinition, TargetingType.ENEMY, 0);
        }

        [Test]
        public void Positive_Create_MultipleEntities()
        {
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
        }
    }
}