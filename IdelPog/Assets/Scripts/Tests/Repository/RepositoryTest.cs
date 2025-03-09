using System;
using IdelPog.Repository;
using IdelPog.Validation;
using IdelPog.Validation.Pipelines.Interfaces;
using Moq;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    public class RepositoryTest
    {
        private IRepository<int, string> _repository;
        private Mock<IRepositoryAsserter> _repositoryAsserterMock;

        private const string VALUE = "TEST STRING";
        private const int KEY = 1;

        [SetUp]
        public void Setup()
        {
            _repositoryAsserterMock = new Mock<IRepositoryAsserter>();
            _repository = new Repository<int, string>(_repositoryAsserterMock.Object);
        }
       
        [Test]
        public void Positive_Add_AddsItem()
        {
            _repository.Add(KEY, VALUE);
            
            string returnedString = _repository.Get(KEY);
            Assert.AreEqual(VALUE, returnedString);
        }

        [Test]
        public void Negative_Add_DuplicateKey_Throws()
        {
            _repository.Add(KEY, VALUE);
            
            Assert.Throws<ArgumentException>(() => _repository.Add(KEY, VALUE));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertObjectNotNull(null))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Add(KEY, null));
        }

        [Test]
        public void Positive_Remove_RemovesItem()
        {
            _repository.Add(KEY, VALUE);
            _repository.Remove(KEY);
            
            bool contains = _repository.Contains(KEY);
            Assert.IsFalse(contains);
        }

        [Test]
        public void Negative_Remove_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertItemIsFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Remove(KEY));
        }

        [Test]
        public void Positive_Get_ReturnsItem()
        {
            _repository.Add(KEY, VALUE);
            
            string returnedString = _repository.Get(KEY);
            Assert.AreEqual(VALUE, returnedString);
        }

        [Test]
        public void Negative_Get_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertItemIsFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Get(KEY));
        }

        [Test]
        public void Positive_Update_UpdatesItem()
        {
            _repository.Add(KEY, VALUE);
            const string newValue = "CHANGED";
            
            _repository.Update(KEY, newValue);
            
            string returnedString = _repository.Get(KEY);
            Assert.AreEqual(newValue, returnedString);
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertItemIsFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Update(KEY, VALUE));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertObjectNotNull(null))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Update(KEY, null));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repository.Add(KEY, VALUE);
            
            bool  contains = _repository.Contains(KEY);
            
            Assert.IsTrue(contains);
        }

        [Test]
        public void Negative_Contains_NotFound_ReturnsFalse()
        {
            bool contains = _repository.Contains(KEY);
            
            Assert.IsFalse(contains);
        }
    }
}