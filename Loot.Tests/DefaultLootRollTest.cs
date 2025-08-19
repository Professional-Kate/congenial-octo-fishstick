using IdelPog.Loot.Random;

namespace Loot.Tests
{
    [TestFixture]
    public class DefaultLootRollTest
    {
        private ILootRoll _lootRoll;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootRoll = new DefaultLootRoll();            
        }

        [Test]
        public void Positive_ExclusiveNextInt_ReturnsInt()
        {
            int roll = _lootRoll.ExclusiveNextInt(0, 1);
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void Positive_SeededLootRoll_ReturnsSameInt()
        {
            const int seed = 365;
            DefaultLootRoll rollOne = new(seed);
            DefaultLootRoll rollTwo = new(seed);
            
            Assert.That(rollOne.ExclusiveNextInt(0, 10), Is.EqualTo(rollTwo.ExclusiveNextInt(0, 10)));
        }
    }
}