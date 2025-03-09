using System;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Validation;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;
using Moq;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class MapperTest
    {
        private Mapper<int> _informationMapper { get; set; }
        private Mock<IAssertFound> _assertFoundMock { get; set; }
        private Mock<IAssertUniqueItem> _assertUniqueMock { get; set; }

        private readonly Information _informationOne = Information.Create("TEST", "TESTING");
        private readonly Information _informationTwo = Information.Create("HELLO", "WORLD");

        [SetUp]
        public void Setup()
        {
            _assertFoundMock = new Mock<IAssertFound>();
            _assertUniqueMock = new Mock<IAssertUniqueItem>();
            
            _informationMapper = new Mapper<int>(_assertFoundMock.Object, _assertUniqueMock.Object);
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
        public void Negative_GetInformation_NotFound_Throws()
        {
            const int badId = -1;
            
            _assertFoundMock.Setup(library => library.AssertItemIsFound(badId, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(badId));
            
            Assert.Throws<NotFoundException>(() => _informationMapper.GetInformation(badId));
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
            _assertUniqueMock.Setup(library => library.AssertUnique(1, It.IsAny<Func<bool>>()))
                .Throws(new DuplicateItemException(1));
            
            Assert.Throws<DuplicateItemException>(() => _informationMapper.AddInformation(1, _informationOne));
        }
    }
}