using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Logging;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Core
{
    [TestFixture]
    public sealed class ReadOnlyCombatantFactoryTest
    {
        private ReadOnlyCombatantFactory _readOnlyCombatantFactory;

        private readonly CombatantEntity _friendlyCombatant = TestCombatantEntityFactory.Create(2, TargetingType.FRIENDLY);
        private readonly CombatantEntity _enemyCombatant = TestCombatantEntityFactory.Create(1, TargetingType.ENEMY);
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _readOnlyCombatantFactory = new ReadOnlyCombatantFactory();
        }
        
        private static void AssertReadOnlyCombatants(ReadOnlyCombatant[] readOnlyCombatants, CombatantEntity[] expectedCombatants)
        {
            for (int i = 0; i < expectedCombatants.Length; i++)
            {
                CombatantEntity expectedCombatant = expectedCombatants[i];
                ReadOnlyCombatant readOnlyCombatant = readOnlyCombatants[i];
                
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(readOnlyCombatant.CombatantID, Is.EqualTo(expectedCombatant.CombatantID));
                    Assert.That(readOnlyCombatant.InstanceID, Is.EqualTo(expectedCombatant.InstanceID));
                    Assert.That(readOnlyCombatant.HealthCard.BaseHealth, Is.EqualTo(expectedCombatant.GetStat(StatType.BASE_HEALTH)));
                    Assert.That(readOnlyCombatant.HealthCard.Health, Is.EqualTo(expectedCombatant.GetStat(StatType.HEALTH)));
                    Assert.That(readOnlyCombatant.AgilityCard.Initiative, Is.EqualTo(expectedCombatant.GetStat(StatType.INITIATIVE)));
                    Assert.That(readOnlyCombatant.AgilityCard.Speed, Is.EqualTo(expectedCombatant.GetStat(StatType.SPEED)));
                    Assert.That(readOnlyCombatant.TargetingType, Is.EqualTo(expectedCombatant.TargetingType));
                }
            }
        }

        [Test]
        public void Positive_Create_ConvertsEntity()
        { 
            ReadOnlyCombatant[] readOnlyCombatants = _readOnlyCombatantFactory.Create([_friendlyCombatant, _enemyCombatant]);
            
            Assert.That(readOnlyCombatants, Has.Length.EqualTo(2));
            AssertReadOnlyCombatants(readOnlyCombatants, [_friendlyCombatant, _enemyCombatant]);
        }

        [Test]
        public void Positive_Create_DoesNotMutateEntities()
        { 
            _readOnlyCombatantFactory.Create([_friendlyCombatant]);
            
            CombatantEntity duplicateCombatant = TestCombatantEntityFactory.Create(2, TargetingType.FRIENDLY);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_friendlyCombatant.GetStat(StatType.BASE_HEALTH), Is.EqualTo(duplicateCombatant.GetStat(StatType.BASE_HEALTH)));
                Assert.That(_friendlyCombatant.GetStat(StatType.HEALTH), Is.EqualTo(duplicateCombatant.GetStat(StatType.HEALTH)));
                Assert.That(_friendlyCombatant.GetStat(StatType.SPEED), Is.EqualTo(duplicateCombatant.GetStat(StatType.SPEED)));
                Assert.That(_friendlyCombatant.GetStat(StatType.INITIATIVE), Is.EqualTo(duplicateCombatant.GetStat(StatType.INITIATIVE)));
            }
        }

        [Test]
        public void Positive_Create_EmptyArray_ReturnsNothing()
        {
            ReadOnlyCombatant[] readOnlyCombatants = _readOnlyCombatantFactory.Create([]);
            
            Assert.That(readOnlyCombatants, Has.Length.EqualTo(0));
        }
    }
}