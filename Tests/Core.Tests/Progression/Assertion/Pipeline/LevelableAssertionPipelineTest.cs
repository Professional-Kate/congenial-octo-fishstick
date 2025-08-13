using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Assertion.Pipelines;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Tests.Progression.Assertion.Pipeline
{
    [TestFixture]
    public class LevelableAssertionPipelineTest
    {
        private ILevelableAssertionPipeline _levelableAssertionPipeline { get; set; }
        private Levelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = new Levelable(1, 0, 10, 0);

            IHandler handler = new ThrowHandler();
            ILevelAssertion levelAssertion = new LevelAssertion(handler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(handler);

            _levelableAssertionPipeline = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
        }

        [Test]
        public void Positive_AssertLevelable_LevelableGood()
        {
            Assert.DoesNotThrow(() => _levelableAssertionPipeline.AssertLevelable(_levelable));
        }

        [Test]
        public void Negative_AssertLevelable_NullLevelable_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _levelableAssertionPipeline.AssertLevelable(null!));
        }

        [Test]
        public void Negative_AssertLevelable_MaxLevel_Throws()
        {
            Levelable levelable = new(100, 0, 10, 0);

            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _levelableAssertionPipeline.AssertLevelable(levelable));
            Assert.Multiple(() =>
            {
                Assert.That(exception.ID, Is.EqualTo(levelable));
                Assert.That(exception.SourceName, Is.EqualTo(nameof(levelable)));
            });
        }
    }
}