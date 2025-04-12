using IdelPog.Engine.Structures;
using IdelPog.Engine.Utilities.Builders;
using NUnit.Framework;

namespace IdelPog.Tests.Models
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
            Assert.That(LEVEL, Is.Not.EqualTo(level));
        }

        [Test]
        public void Positive_LevelUp_CallsAction()
        {
            _levelable.LevelUp();      
        }
    }
}