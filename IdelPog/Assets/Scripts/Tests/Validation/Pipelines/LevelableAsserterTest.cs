using System;
using IdelPog.Structures.Builders;
using IdelPog.Structures.Models.Levelable;
using IdelPog.Validation;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Pipelines;
using IdelPog.Validation.Pipelines.Interfaces;
using NUnit.Framework;

namespace Tests.Validation.Pipelines
{
    [TestFixture]
    public class LevelableAsserterTest
    {
        private ILevelableAsserter _levelableAsserter { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = LevelableBuilder.Builder()
                .Experience(0)
                .Level(1)
                .ExperiencePerAction(0)
                .NextLevelExperience(10)
                .Build();
            
            IHandler handler = new ThrowHandler();
            IAssertUnderMaxLevel assertUnderMaxLevel = new AssertUnderMaxLevel(handler);
            IAssertNotNull assertNotNull = new AssertNotNull(handler);
            IAssertPositive assertPositive = new AssertPositive(handler);
            
            _levelableAsserter = new LevelableAsserter(assertUnderMaxLevel, assertNotNull, assertPositive);
        }

        [Test]
        public void Positive_AssertLevelable_LevelableGood()
        {
            Assert.DoesNotThrow(() => _levelableAsserter.AssertLevelable(_levelable));
        }

        [Test]
        public void Negative_AssertLevelable_NullLevelable_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _levelableAsserter.AssertLevelable(null));
        }

        [Test]
        public void Negative_AssertLevelable_MaxLevel_Throws()
        {
            ILevelable levelable = LevelableBuilder.Builder()
                .Level(100)
                .Build();
            
            Assert.Throws<MaxLevelException>(() => _levelableAsserter.AssertLevelable(levelable));
        }

        [Test]
        public void Positive_AssertLevelable_NegativeExperiencePerAction_Throws()
        {
            ILevelable levelable = LevelableBuilder.Builder()
                .ExperiencePerAction(-1)
                .Build();
            
            Assert.Throws<NegativeNumberException>(() => _levelableAsserter.AssertLevelable(levelable));
        }
    }
}