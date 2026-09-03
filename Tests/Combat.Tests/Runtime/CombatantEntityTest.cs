using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class CombatantEntityTest
    {
        private CombatantEntity _combatantEntity;

        private HealthComponent _healthComponent;

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = TestCombatantEntityFactory.Create(0, TargetingType.FRIENDLY);
            _healthComponent = _combatantEntity.GetComponent<HealthComponent>();
        }

        private void UpdateCombatantStats(HealthComponent healthComponent)
        { 
            _combatantEntity.ReplaceComponent(healthComponent);
        }

        private HealthComponent GetComponent()
        { 
            return _combatantEntity.GetComponent<HealthComponent>();
        }

        private static void VerifyComponent(HealthComponent expectedHealth, HealthComponent component)
        {
            Assert.That(component, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_UpdateCombatantStats_UpdatesStats()
        { 
            UpdateCombatantStats(_healthComponent with { Health = 5 });
            
            HealthComponent component = GetComponent();

            VerifyComponent(_healthComponent with { Health = 5 }, component);
        }

        [Test]
        public void Positive_UpdateCombatantStats_MultipleTimes_UpdatesStats()
        {
            UpdateCombatantStats(new HealthComponent { Health = 5 });
            VerifyComponent(new HealthComponent { Health = 5 }, GetComponent());
            
            UpdateCombatantStats(new HealthComponent { Health = 22 });
            VerifyComponent(new HealthComponent { Health = 22 }, GetComponent());
        }
    }
}