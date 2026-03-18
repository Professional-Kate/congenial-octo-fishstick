using IdelPog.Combat;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatTest
    {
        [SetUp]
        public void Setup()
        { 
            CombatBootstrapper.SetupCombat();
        }
    }
}