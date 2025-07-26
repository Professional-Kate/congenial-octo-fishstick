using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Common.Tests
{
    [TestFixture]
    public class RepositoryAsserterTest
    {
        private IRepositoryAsserter _repositoryAsserter { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            IHandler handler = new ThrowHandler();
            IFoundAssertion foundAssertion = new FoundAssertion(handler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(handler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(handler);

            _repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
        }

        [Test]
        public void Positive_AssertUnique_PassedFalse()
        {
            Assert.DoesNotThrow(() => _repositoryAsserter.AssertUnique(1, false));
        }

        [Test]
        public void Negative_AssertUnique_PassedTrue_Throws()
        {
            Assert.Throws<DuplicateItemException>(() => _repositoryAsserter.AssertUnique(1, true));
        }

        [Test]
        public void Negative_AssertUnique_PassedNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repositoryAsserter.AssertUnique<string>(null!, false));
        }

        [Test]
        public void Positive_AssertFound_PassedTrue()
        {
            Assert.DoesNotThrow(() => _repositoryAsserter.AssertFound(1, true));
        }

        [Test]
        public void Negative_AssertFound_PassedFalse_Throws()
        {
             NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _repositoryAsserter.AssertFound(1, false));
             Assert.That(exception.Key, Is.EqualTo(1));
        }

        [Test]
        public void Negative_AssertFound_PassedNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repositoryAsserter.AssertFound<string>(null!, true));
        }
    }
}