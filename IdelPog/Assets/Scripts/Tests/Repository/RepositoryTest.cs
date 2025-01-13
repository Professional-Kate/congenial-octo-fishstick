using System;
using IdelPog.Exceptions;
using IdelPog.Repository;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    public class RepositoryTest
    {
        private IRepository<int, string> _repository;

        private const string Value = "TEST STRING";
        private const int Key = 1;

        [SetUp]
        public void Setup()
        {
            _repository = new Repository<int, string>();
        }

        [Test]
        public void Positive_Add_AddsItem()
        {
            _repository.Add(Key, Value);
            
            string returnedString = _repository.Get(Key);
            Assert.AreEqual(Value, returnedString);
        }

        [Test]
        public void Negative_Add_DuplicateKey_Throws()
        {
            _repository.Add(Key, Value);
            
            Assert.Throws<ArgumentException>(() => _repository.Add(Key, Value));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(Key, null));
        }

        [Test]
        public void Positive_Remove_RemovesItem()
        {
            _repository.Add(Key, Value);
            _repository.Remove(Key);
            
            Assert.Throws<NotFoundException>(() => _repository.Get(Key));
        }

        [Test]
        public void Negative_Remove_NonExisting_Throws()
        {
            Assert.Throws<NotFoundException>(() => _repository.Remove(Key));
        }

        [Test]
        public void Positive_Get_ReturnsItem()
        {
            _repository.Add(Key, Value);
            
            string returnedString = _repository.Get(Key);
            Assert.AreEqual(Value, returnedString);
        }

        [Test]
        public void Negative_Get_NonExisting_Throws()
        {
            Assert.Throws<NotFoundException>(() => _repository.Get(Key));
        }

        [Test]
        public void Positive_Update_UpdatesItem()
        {
            _repository.Add(Key, Value);
            const string newValue = "CHANGED";
            
            _repository.Update(Key, newValue);
            
            string returnedString = _repository.Get(Key);
            Assert.AreEqual(newValue, returnedString);
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            Assert.Throws<NotFoundException>(() => _repository.Update(Key, Value));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Update(Key, null));
        }
    }
}