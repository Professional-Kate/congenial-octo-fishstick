using IdelPog.Combat.Combatant.Runtime.Component;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class RetaliationComponentTest
    {
        private RetaliationComponent _retaliationComponent;
        
        private const byte CAPACITY = 3;
        private readonly CombatantDamageComponent _combatantDamageComponent = new()
        {
            CombatantID = 1,
            DamageValue = 10
        };

        [SetUp]
        public void Setup()
        {
            _retaliationComponent = new RetaliationComponent
            {
                Capacity = CAPACITY
            };
        }

        private static void AssertCombatantDamageComponent(CombatantDamageComponent combatantDamageComponent, CombatantDamageComponent expectedComponent)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantDamageComponent.CombatantID, Is.EqualTo(expectedComponent.CombatantID));
                Assert.That(combatantDamageComponent.DamageValue, Is.EqualTo(expectedComponent.DamageValue));
            }
        }
        
        private void AssertTryDequeue(CombatantDamageComponent expectedComponent, bool expectedSuccess)
        {
            bool successful = _retaliationComponent.TryDequeue(out CombatantDamageComponent combatantDamageComponent);
            
            Assert.That(successful, Is.EqualTo(expectedSuccess));
            AssertCombatantDamageComponent(combatantDamageComponent, expectedComponent);
        }

        [Test]
        public void Positive_Enqueue_EnqueuesComponent()
        { 
            _retaliationComponent.Enqueue(_combatantDamageComponent);
            
            AssertTryDequeue(_combatantDamageComponent, true);
        }

        [Test]
        public void Positive_Enqueue_CanEnqueueTillMax()
        {
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 2 });
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 3 });
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 4 });
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 5 });
            
            AssertTryDequeue(_combatantDamageComponent with { CombatantID = 3 }, true);
        }

        [Test]
        public void Positive_TryDequeue_NoComponents_ReturnsFalse()
        {
            bool successful = _retaliationComponent.TryDequeue(out CombatantDamageComponent combatantDamageComponent);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(successful, Is.False);
                Assert.That(combatantDamageComponent, Is.Default);
            }
        }

        [Test]
        public void Positive_TryDequeue_ReturnsAddedComponent()
        {
            _retaliationComponent.Enqueue(_combatantDamageComponent);
            
            AssertTryDequeue(_combatantDamageComponent, true);
        }

        [Test]
        public void Positive_TryDequeue_DequeuesSeries()
        {
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 3 });
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 4 });
            _retaliationComponent.Enqueue(_combatantDamageComponent with { CombatantID = 5 });

            AssertTryDequeue(_combatantDamageComponent with { CombatantID = 3 }, true);
            AssertTryDequeue(_combatantDamageComponent with { CombatantID = 4 }, true);
            AssertTryDequeue(_combatantDamageComponent with { CombatantID = 5 }, true);
        }
    }
}