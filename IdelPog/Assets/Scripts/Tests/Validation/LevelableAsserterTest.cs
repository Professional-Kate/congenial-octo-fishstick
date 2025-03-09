using System;
using IdelPog.Model;
using IdelPog.Validation;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Pipelines;
using IdelPog.Validation.Pipelines.Interfaces;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Validation
{
    [TestFixture]
    public class LevelableAsserterTest
    {
        private ILevelableAsserter _levelableAsserter { get; set; }
        private Job _job { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _job = JobFactory.CreateMining();
            
            IHandler handler = new ThrowHandler();
            IAssertUnderMaxLevel assertUnderMaxLevel = new AssertUnderMaxLevel(handler);
            IAssertNotNull assertNotNull = new AssertNotNull(handler);
            IAssertPositive assertPositive = new AssertPositive(handler);
            
            _levelableAsserter = new LevelableAsserter(assertUnderMaxLevel, assertNotNull, assertPositive);
        }

        private Job CloneJob()
        {
            Job clonedJob = _job.Clone() as Job;
            if (clonedJob == null)
            {
                Assert.Fail("cloned job is null!");
            }
            
            return clonedJob;
        }

        [Test]
        public void Positive_AssertLevelable_LevelableGood()
        {
            Assert.DoesNotThrow(() => _levelableAsserter.AssertLevelable(_job));
        }

        [Test]
        public void Negative_AssertLevelable_NullLevelable_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _levelableAsserter.AssertLevelable(null));
        }

        [Test]
        public void Negative_AssertLevelable_MaxLevel_Throws()
        {
            Job badJob = CloneJob();
            badJob.Setup(100, 1, 1, 1);
            
            Assert.Throws<MaxLevelException>(() => _levelableAsserter.AssertLevelable(badJob));
        }

        [Test]
        public void Positive_AssertLevelable_NegativeExperiencePerAction_Throws()
        {
            Job badJob = CloneJob();
            badJob.Setup(10, 1, 1, -1);
            
            Assert.Throws<NegativeNumberException>(() => _levelableAsserter.AssertLevelable(badJob));
        }
    }
}