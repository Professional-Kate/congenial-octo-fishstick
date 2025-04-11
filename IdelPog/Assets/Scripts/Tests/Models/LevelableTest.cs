using IdelPog.Structures.Builders;
using IdelPog.Structures.Models.Levelable;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class LevelableTest
    {
        private ILevelable _levelable { get; set; }

        private const byte LEVEL = 5;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = LevelableBuilder.Builder()
                .Level(LEVEL)
                .OnLevelUp(AssertLevelUp)
                .Build();
        }

        private static void AssertLevelUp(byte level)
        {
            Assert.AreNotEqual(LEVEL, level);
        }

        [Test]
        public void Positive_LevelUp_CallsAction()
        {
            _levelable.LevelUp();
        }
    }
}