using System.Collections.Immutable;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Tests.Stat.Runtime
{
    [TestFixture]
    public sealed class StatsComponentTest
    {
        private StatsComponent _statsComponent;

        private readonly StatComponent _healthStat = new() { StatType = StatType.HEALTH, Stat = 12 };
        private readonly StatComponent _speedStat = new() { StatType = StatType.SPEED, Stat = 25 };
        private readonly StatComponent _abilityDamageStat = new() { StatType = StatType.ABILITY_DAMAGE, Stat = 94 };

        [SetUp]
        public void Setup()
        { 
            _statsComponent = new StatsComponent { StatComponents = [_healthStat, _speedStat, _abilityDamageStat] };
        }

        private static void AssertStat(uint stat, uint expectedStat)
        {
            Assert.That(stat, Is.EqualTo(expectedStat));
        }

        [Test]
        public void Positive_GetStat_ReturnsCorrectStat()
        {
            uint health = _statsComponent.GetStat(StatType.HEALTH);
            
            AssertStat(health, _healthStat.Stat);
        }
        
        [Test]
        public void Positive_GetStat_DuplicateStat_OnlyReturnsFirstFound()
        {
            StatsComponent oopsOnlySpeed = new() { StatComponents = [_speedStat, _speedStat with { Stat = uint.MaxValue }] };
            
            uint speed = oopsOnlySpeed.GetStat(StatType.SPEED);
            
            AssertStat(speed, _speedStat.Stat);
        }

        [Test]
        public void Negative_GetStat_StatNotFound_Throws()
        { 
            Assert.Throws<KeyNotFoundException>(() => _statsComponent.GetStat(StatType.INITIATIVE));
        }

        [Test]
        public void Positive_ReplaceStat_ReplacesStat()
        {
            AssertStat(_statsComponent.GetStat(StatType.SPEED), _speedStat.Stat);
            
            _statsComponent.ReplaceStat(_speedStat.StatType, 100);

            AssertStat(_statsComponent.GetStat(StatType.SPEED), 100);
        }

        [Test]
        public void Positive_ReplaceStat_DuplicateStat_OnlyChangesFirstFound()
        {
            StatsComponent oopsOnlyHealth = new() { StatComponents = [_healthStat, _healthStat with { Stat = uint.MaxValue }] }; 
            
            oopsOnlyHealth.ReplaceStat(StatType.HEALTH, uint.MinValue);
            
            AssertStat(oopsOnlyHealth.GetStat(StatType.HEALTH), uint.MinValue);
        }

        [Test]
        public void Negative_ReplaceStat_StatNotFound_Throws()
        { 
            Assert.Throws<KeyNotFoundException>(() => _statsComponent.ReplaceStat(StatType.INITIATIVE, 1));
        }

        [Test]
        public void Positive_GetAllStats_ReturnsEverything()
        {
            ImmutableArray<StatComponent> allStats = _statsComponent.GetAllStats();
            
            Assert.That(allStats, Is.EqualTo([_healthStat, _speedStat, _abilityDamageStat]));
        }

        [Test]
        public void Positive_GetAllStats_ReturnsDuplicates()
        {
            StatsComponent oopsOnlyAbilityDamage = new() { StatComponents = [_abilityDamageStat, _abilityDamageStat with { Stat = uint.MaxValue }] };
            
            ImmutableArray<StatComponent> allStats = oopsOnlyAbilityDamage.GetAllStats();
            
            Assert.That(allStats, Is.EqualTo([_abilityDamageStat, _abilityDamageStat with { Stat = uint.MaxValue }]));
        }

        [Test]
        public void Positive_GetAllStats_ReturnsExactCollection_EveryTime()
        {
            ImmutableArray<StatComponent> allStats = _statsComponent.GetAllStats();
            Assert.That(allStats, Is.EqualTo(_statsComponent.GetAllStats()));
        }

        [Test]
        public void Positive_GetAllStats_EmptyStats_ReturnsEmptyArray()
        {
            StatsComponent emptyComponent = new() { StatComponents = [] };
            
            ImmutableArray<StatComponent> allStats = emptyComponent.GetAllStats();
            
            Assert.That(allStats, Is.Not.Default);
            Assert.That(allStats, Is.Empty);
        }

        [Test]
        public void Positive_GetAllStats_ReturnsNewCollection()
        {
            ImmutableArray<StatComponent> allStats = _statsComponent.GetAllStats();
            Assert.That(allStats[0], Is.EqualTo(_healthStat));
            
            _statsComponent.ReplaceStat(StatType.HEALTH, uint.MaxValue);
            Assert.That(allStats[0], Is.EqualTo(_healthStat));
        }

        [Test]
        public void Positive_GetAllStats_ReplaceStat_ThenGetAll_ReturnsNewState()
        {
            _statsComponent.ReplaceStat(StatType.HEALTH, uint.MaxValue);
            
            ImmutableArray<StatComponent> allStats = _statsComponent.GetAllStats();
            
            Assert.That(allStats[0], Is.EqualTo(_healthStat with { Stat = uint.MaxValue }));
        }
    }
}