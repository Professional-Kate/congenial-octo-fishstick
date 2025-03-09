using IdelPog.Model;
using IdelPog.Validation;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Validation
{
    [TestFixture]
    public class AssertUnderMaxLevelTest
    {
        private IAssertUnderMaxLevel _assertUnderMaxLevel { get; set; }
        private Job _job { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _job = JobFactory.CreateFarming();
            _assertUnderMaxLevel = new AssertUnderMaxLevel(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertLevelIsUnderMax_LevelUnderMax()
        {
            _job.Setup(99, 1, 1, 1);
            Assert.DoesNotThrow(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(_job));
        }

        [Test]
        public void Negative_AssertLevelIsUnderMax_LevelIsMax_Throws()
        {
            _job.Setup(100, 1, 1, 1);
            Assert.Throws<MaxLevelException>(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(_job));
        }
    }
}