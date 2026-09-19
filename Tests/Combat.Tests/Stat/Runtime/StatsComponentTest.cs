using System.Collections.Immutable;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Tests.Stat.Runtime
{
    [TestFixture]
    public sealed class StatsComponentTest
    {
        private StatsComponent _statsComponent;

        private readonly StatComponent _healthStat = new() { StatID = 0, Value = 12 };
        private readonly StatComponent _speedStat = new() { StatID = 1, Value = 25 };
        private readonly StatComponent _abilityDamageStat = new() { StatID = 2, Value = 94 };

        [SetUp]
        public void Setup()
        { 
            _statsComponent = new StatsComponent { StatComponents = [_healthStat, _speedStat, _abilityDamageStat] };
        }

        private static void AssertStat(uint stat, uint expectedStatValue)
        {
            Assert.That(stat, Is.EqualTo(expectedStatValue));
        }

        [Test]
        public void Positive_GetStat_ReturnsCorrectStat()
        {
            uint health = _statsComponent.GetStat(_healthStat.StatID);
            
            AssertStat(health, _healthStat.Value);
        }
        
        [Test]
        public void Positive_GetStat_DuplicateStat_OnlyReturnsFirstFound()
        {
            StatsComponent oopsOnlySpeed = new() { StatComponents = [_speedStat, _speedStat with { Value = uint.MaxValue }] };
            
            uint speed = oopsOnlySpeed.GetStat(_speedStat.StatID);
            
            AssertStat(speed, _speedStat.Value);
        }

        [Test]
        public void Negative_GetStat_StatNotFound_Throws()
        { 
            Assert.Throws<KeyNotFoundException>(() => _statsComponent.GetStat(3));
        }

        [Test]
        public void Positive_ReplaceStat_ReplacesStat()
        {
            AssertStat(_statsComponent.GetStat(_speedStat.StatID), _speedStat.Value);
            
            _statsComponent.ReplaceStat(_speedStat.StatID, 100);

            AssertStat(_statsComponent.GetStat(_speedStat.StatID), 100);
        }

        [Test]
        public void Positive_ReplaceStat_DuplicateStat_OnlyChangesFirstFound()
        {
            StatsComponent oopsOnlyHealth = new() { StatComponents = [_healthStat, _healthStat with { Value = uint.MaxValue }] }; 
            
            oopsOnlyHealth.ReplaceStat(_healthStat.StatID, uint.MinValue);
            
            AssertStat(oopsOnlyHealth.GetStat(_healthStat.StatID), uint.MinValue);
        }

        [Test]
        public void Negative_ReplaceStat_StatNotFound_Throws()
        { 
            Assert.Throws<KeyNotFoundException>(() => _statsComponent.ReplaceStat(3, 1));
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
            StatsComponent oopsOnlyAbilityDamage = new() { StatComponents = [_abilityDamageStat, _abilityDamageStat with { Value = uint.MaxValue }] };
            
            ImmutableArray<StatComponent> allStats = oopsOnlyAbilityDamage.GetAllStats();
            
            Assert.That(allStats, Is.EqualTo([_abilityDamageStat, _abilityDamageStat with { Value = uint.MaxValue }]));
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
            Assert.That(allStats[_healthStat.StatID], Is.EqualTo(_healthStat));
            
            _statsComponent.ReplaceStat(_healthStat.StatID, uint.MaxValue);
            Assert.That(allStats[_healthStat.StatID], Is.EqualTo(_healthStat));
        }

        [Test]
        public void Positive_GetAllStats_ReplaceStat_ThenGetAll_ReturnsNewState()
        {
            _statsComponent.ReplaceStat(_healthStat.StatID, uint.MaxValue);
            
            ImmutableArray<StatComponent> allStats = _statsComponent.GetAllStats();
            
            Assert.That(allStats[_healthStat.StatID], Is.EqualTo(_healthStat with { Value = uint.MaxValue }));
        }
    }
}