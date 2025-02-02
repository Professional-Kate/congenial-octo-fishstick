using System;
using IdelPog.Exceptions;
using IdelPog.Repository;
using IdelPog.Structures.Enums;
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
            _repository = Repository<int, string>.GetInstance();
            _repository.Clear();
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

        [TestCase(CurrencyType.NO_TYPE)]
        [TestCase(JobType.NO_TYPE)]
        public void Negative_AddBadKey_Throws<T>(T type)
        {
            Assert.Throws<NoTypeException>(() => _repository.Add(type.GetHashCode(), "FAIL"));
        }
        
        [TestCase(CurrencyType.NO_TYPE)]
        [TestCase(JobType.NO_TYPE)]
        public void Negative_RemoveBadKey_Throws<T>(T type)
        {
            Assert.Throws<NoTypeException>(() => _repository.Remove(type.GetHashCode()));
        }
        
        [TestCase(CurrencyType.NO_TYPE)]
        [TestCase(JobType.NO_TYPE)]
        public void Negative_GetBadKey_Throws<T>(T type)
        {
            Assert.Throws<NoTypeException>(() => _repository.Update(type.GetHashCode(), "FAIL"));
        }
        
        [TestCase(CurrencyType.NO_TYPE)]
        [TestCase(JobType.NO_TYPE)]
        public void Negative_UpdateBadKey_Throws<T>(T type)
        {
            Assert.Throws<NoTypeException>(() => _repository.Get(type.GetHashCode()));
        }
        
        [TestCase(CurrencyType.NO_TYPE)]
        [TestCase(JobType.NO_TYPE)]
        public void Negative_ContainsBadKey_Throws<T>(T type)
        {
            Assert.Throws<NoTypeException>(() => _repository.Contains(type.GetHashCode()));
        }
        

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repository.Add(Key, Value);
            
            bool  contains = _repository.Contains(Key);
            
            Assert.IsTrue(contains);
        }

        [Test]
        public void Negative_Contains_NotFound_ReturnsFalse()
        {
            bool  contains = _repository.Contains(Key);
            
            Assert.IsFalse(contains);
        }

        [Test]
        public void Positive_Clear_Clears_Repository()
        {
            _repository.Add(Key, Value);
            
            _repository.Clear();
            
            bool contains = _repository.Contains(Key);
            Assert.IsFalse(contains);
        }
    }
}