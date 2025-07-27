using IdelPog.SimulationEngine.Assertions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPogTests.Assertions
{
    [TestFixture]
    public class LevelAssertionTest
    {
        private ILevelAssertion _levelAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelAssertion = new LevelAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertBelowMaxLevel_LevelUnderMax_NoThrow()
        {
            Levelable levelable = new(99, 1, 10, 0);

            Assert.DoesNotThrow(() => _levelAssertion.AssertBelowMaxLevel(levelable));
        }

        [Test]
        public void Negative_AssertBelowMaxLevel_LevelIsMax_Throws()
        {
            Levelable levelable = new(100, 1, 1, 1);

            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _levelAssertion.AssertBelowMaxLevel(levelable));
            Assert.Multiple(() =>
            {
                Assert.That(exception.ID, Is.EqualTo(levelable));
                Assert.That(exception.SourceName, Is.EqualTo(nameof(levelable)));
            });
        }
    }
}