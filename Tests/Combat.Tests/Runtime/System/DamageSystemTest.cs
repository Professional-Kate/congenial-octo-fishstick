using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        
        private CombatantEntity _targetEntity;
        private StatCard _targetEntityStats;
        private StatCard _attackerStats;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystem = new DamageSystem();

            _targetEntityStats = new StatCard { Attack = 10, Health = 20, Speed = 5 };
            _attackerStats = new StatCard { Attack = 5, Health = 10, Speed = 5 };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = CombatantEntityFactory.CreateCombatantEntity(0, _targetEntityStats);
        }

        private static void AssertNewHealth(uint newHealth, uint expectedHealth)
        { 
            Assert.That(newHealth, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _attackerStats);
            
            AssertNewHealth(newHealth,_targetEntityStats.Health - _attackerStats.Attack);
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _attackerStats with { Attack = 200 });
            
            AssertNewHealth(newHealth, 0);
        }
    }
}