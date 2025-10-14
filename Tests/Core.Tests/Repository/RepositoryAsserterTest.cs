using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Repository
{
    [TestFixture]
    public sealed class RepositoryAsserterTest
    {
        private IRepositoryAsserter _repositoryAsserter { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            IFoundAssertion foundAssertion = new FoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();

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
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _repositoryAsserter.AssertUnique(1, true));
            Assert.That(exception.ID, Is.EqualTo(1));
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