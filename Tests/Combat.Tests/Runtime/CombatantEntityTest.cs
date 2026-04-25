using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class CombatantEntityTest
    {
        private CombatantEntity _combatantEntity;

        private CombatantStatsComponent _combatantStatsComponent;

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = TestCombatantEntityFactory.CreateCombatantEntity(0);
            _combatantStatsComponent = _combatantEntity.GetComponent<CombatantStatsComponent>();
        }

        private void UpdateCombatantStats(CombatantStatsComponent combatantStatsComponent)
        { 
            _combatantEntity.UpdateCombatantStats(combatantStatsComponent);
        }

        private CombatantStatsComponent GetComponent()
        { 
            return _combatantEntity.GetComponent<CombatantStatsComponent>();
        }

        private static void VerifyComponent(CombatantStatsComponent expectedStats, CombatantStatsComponent component)
        {
            Assert.That(component, Is.EqualTo(expectedStats));
        }

        [Test]
        public void Positive_UpdateCombatantStats_UpdatesStats()
        { 
            UpdateCombatantStats(_combatantStatsComponent with { Health = 5 });
            
            CombatantStatsComponent component = GetComponent();

            VerifyComponent(_combatantStatsComponent with { Health = 5 }, component);
        }

        [Test]
        public void Positive_UpdateCombatantStats_MultipleTimes_UpdatesStats()
        {
            UpdateCombatantStats(_combatantStatsComponent with { Health = 5 });
            VerifyComponent(_combatantStatsComponent with { Health = 5 }, GetComponent());
            
            UpdateCombatantStats(_combatantStatsComponent with { Health = 22, Attack = 15 });
            VerifyComponent(_combatantStatsComponent with { Health = 22, Attack = 15 }, GetComponent());
        }
    }
}