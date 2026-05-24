using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

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
            _combatantEntity.UpdateCombatantStats(statsComponent);
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
            UpdateCombatantStats(_statsComponent with { Health = 5 });
            VerifyComponent(_statsComponent with { Health = 5 }, GetComponent());
            
            UpdateCombatantStats(_statsComponent with { Health = 22, Attack = 15 });
            VerifyComponent(_statsComponent with { Health = 22, Attack = 15 }, GetComponent());
        }
    }
}