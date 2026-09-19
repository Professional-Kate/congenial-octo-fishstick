using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class RetaliationComponentTest
    {
        private RetaliationComponent _retaliationComponent;
        
        private const byte CAPACITY = 3;
        private readonly CombatantDamaged _combatantDamaged = new()
        {
            InitiatingCombatant = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY),
            DamageValue = 10
        };

        private readonly CombatantEntity _damagedCombatant = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);

        [SetUp]
        public void Setup()
        {
            _retaliationComponent = new RetaliationComponent
            {
                Capacity = CAPACITY
            };
        }

        private static void AssertCombatantDamageComponent(CombatantDamaged combatantDamaged, CombatantDamaged expectedComponent)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantDamaged.InitiatingCombatant, Is.EqualTo(expectedComponent.InitiatingCombatant));
                Assert.That(combatantDamaged.DamageValue, Is.EqualTo(expectedComponent.DamageValue));
            }
        }
        
        private void AssertTryDequeue(CombatantDamaged expectedComponent, bool expectedSuccess)
        {
            bool successful = _retaliationComponent.TryDequeue(out CombatantDamaged combatantDamageComponent);
            
            Assert.That(successful, Is.EqualTo(expectedSuccess));
            AssertCombatantDamageComponent(combatantDamageComponent, expectedComponent);
        }

        [Test]
        public void Positive_Enqueue_EnqueuesComponent()
        { 
            _retaliationComponent.Enqueue(_combatantDamaged);
            
            AssertTryDequeue(_combatantDamaged, true);
        }

        [Test]
        public void Positive_Enqueue_CanEnqueueTillMax()
        {
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 1 } });
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 2 } });
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 3 } });
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 4 } });
            
            AssertTryDequeue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 2 }}, true);
        }

        [Test]
        public void Positive_TryDequeue_NoComponents_ReturnsFalse()
        {
            bool successful = _retaliationComponent.TryDequeue(out CombatantDamaged combatantDamageComponent);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(successful, Is.False);
                Assert.That(combatantDamageComponent, Is.Default);
            }
        }

        [Test]
        public void Positive_TryDequeue_ReturnsAddedComponent()
        {
            _retaliationComponent.Enqueue(_combatantDamaged);
            
            AssertTryDequeue(_combatantDamaged, true);
        }

        [Test]
        public void Positive_TryDequeue_DequeuesSeries()
        {
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 3 }});
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 4 }});
            _retaliationComponent.Enqueue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 5 }});

            AssertTryDequeue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 3 }}, true);
            AssertTryDequeue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 4 }}, true);
            AssertTryDequeue(_combatantDamaged with { InitiatingCombatant = _damagedCombatant with { InstanceID = 5 }}, true);
        }
    }
}