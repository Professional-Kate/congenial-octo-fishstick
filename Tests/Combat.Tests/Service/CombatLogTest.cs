using IdelPog.Combat.Service;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatLogTest
    {
        private CombatLog _combatLog;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatLog = new CombatLog();
        }
    }
}