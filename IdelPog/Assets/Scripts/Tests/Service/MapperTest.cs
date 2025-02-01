using System;
using IdelPog.Exceptions;
using IdelPog.Service;
using IdelPog.Structures;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class MapperTest
    {
        private Mapper<int> _informationMapper { get; set; }

        private readonly Information _informationOne = Information.Create("TEST", "TESTING");
        private readonly Information _informationTwo = Information.Create("HELLO", "WORLD");

        [SetUp]
        public void Setup()
        {
            _informationMapper = new Mapper<int>();
            _informationMapper.AddInformation(1, _informationOne);
            _informationMapper.AddInformation(2, _informationTwo);
        }

        [Test]
        public void Positive_GetInformation_Returns_Information()
        {
            Information returnedInfo = _informationMapper.GetInformation(1);
            
            Assert.AreEqual(_informationOne, returnedInfo);
            Assert.AreEqual(_informationOne.Description, returnedInfo.Description);
            Assert.AreEqual(_informationOne.Name, returnedInfo.Name);
        }

        [Test]
        public void Negative_GetInformation_NoTypeKey_Throws()
        {
            Assert.Throws<NoTypeException>(() => _informationMapper.GetInformation(0));
        }

        [Test]
        public void Positive_AddInformation_Adds_Information()
        {
            Information newInformation = Information.Create("AAAAA", "AAA");
            _informationMapper.AddInformation(3, newInformation);
            
            Information returnedInfo = _informationMapper.GetInformation(3);
            
            Assert.AreEqual(newInformation, returnedInfo);
            Assert.AreEqual(newInformation.Description, returnedInfo.Description);
            Assert.AreEqual(newInformation.Name, returnedInfo.Name);
        }

        [Test]
        public void Negative_AddInformation_KeyAlreadyExists_Throws()
        {
            Assert.Throws<ArgumentException>(() => _informationMapper.AddInformation(1, _informationOne));
        }

        [Test]
        public void Negative_AddInformation_NoTypeKey_Throws()
        {
            Assert.Throws<NoTypeException>(() => _informationMapper.AddInformation(0, _informationOne));
        }
    }
}