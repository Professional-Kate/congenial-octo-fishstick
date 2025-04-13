using IdelPog.Engine.Structures.Models;

namespace IdelPogTests.Models
{
    [TestFixture]
    public class LevelableTest
    {
        private ILevelable _levelable { get; set; }

        private const byte LEVEL = 5;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = new Levelable(LEVEL, 0, 10, 0);
            _levelable.OnLevelUp += AssertLevelUp;
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