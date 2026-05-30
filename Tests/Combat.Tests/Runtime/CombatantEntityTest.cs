using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class CombatantEntityTest
    {
        private CombatantEntity _combatantEntity;

        private StatsComponent _statsComponent;

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(0);
            _statsComponent = _combatantEntity.GetComponent<StatsComponent>();
        }

        private void UpdateCombatantStats(StatsComponent statsComponent)
        { 
            _combatantEntity.ReplaceComponent(statsComponent);
        }

        private StatsComponent GetComponent()
        { 
            return _combatantEntity.GetComponent<StatsComponent>();
        }

        private static void VerifyComponent(StatsComponent expectedStats, StatsComponent component)
        {
            Assert.That(component, Is.EqualTo(expectedStats));
        }

        [Test]
        public void Positive_UpdateCombatantStats_UpdatesStats()
        { 
            UpdateCombatantStats(_statsComponent with { Health = 5 });
            
            StatsComponent component = GetComponent();

            VerifyComponent(_statsComponent with { Health = 5 }, component);
        }

        [Test]
        public void Positive_UpdateCombatantStats_MultipleTimes_UpdatesStats()
        {
            UpdateCombatantStats(new StatsComponent { Health = 5 });
            VerifyComponent(new StatsComponent { Health = 5 }, GetComponent());
            
            UpdateCombatantStats(new StatsComponent { Health = 22 });
            VerifyComponent(new StatsComponent { Health = 22 }, GetComponent());
        }
    }
}