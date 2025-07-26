using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class MapperTest
    {
        private Mapper<int> _informationMapper { get; set; }
        private IFoundAssertion _assertFound { get; set; }
        private IUniqueAssertion _assertUnique { get; set; }

        private readonly Information _informationOne = new("TEST", "TESTING");
        private readonly Information _informationTwo = new("HELLO", "WORLD");

        [SetUp]
        public void Setup()
        {
            _assertFound = new FoundAssertion(new ThrowHandler());
            _assertUnique = new UniqueAssertion(new ThrowHandler());

            _informationMapper = new Mapper<int>(_assertFound, _assertUnique);
            _informationMapper.AddInformation(1, _informationOne);
            _informationMapper.AddInformation(2, _informationTwo);
        }

        [Test]
        public void Positive_GetInformation_Returns_Information()
        {
            Information returnedInfo = _informationMapper.GetInformation(1);

            Assert.That(_informationOne, Is.EqualTo(returnedInfo));
            Assert.That(_informationOne.Description, Is.EqualTo(returnedInfo.Description));
            Assert.That(_informationOne.Name, Is.EqualTo(returnedInfo.Name));
        }

        [Test]
        public void Negative_GetInformation_NotFound_Throws()
        {
            const int badId = -1;

            Assert.Throws<NotFoundException>(() => _informationMapper.GetInformation(badId));
        }

        [Test]
        public void Positive_AddInformation_Adds_Information()
        {
            Information newInformation = new("AAAAA", "AAA");
            _informationMapper.AddInformation(3, newInformation);

            Information returnedInfo = _informationMapper.GetInformation(3);

            Assert.That(newInformation, Is.EqualTo(returnedInfo));
            Assert.That(newInformation.Description, Is.EqualTo(returnedInfo.Description));
            Assert.That(newInformation.Name, Is.EqualTo(returnedInfo.Name));
        }

        [Test]
        public void Negative_AddInformation_KeyAlreadyExists_Throws()
        {
            Assert.Throws<DuplicateItemException>(() => _informationMapper.AddInformation(1, _informationOne));
        }
    }
}