using ContentHydrator.Assertions;
using ContentHydrator.Converters;
using ContentHydrator.DTO;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentHydratorTests.Converters
{
    [TestFixture]
    public class JobConverterTest
    {
        private JobConverter _converter { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }
        private const string JOB = "Mining";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _converter = new JobConverter(new AssertNotNull(_handlerMock.Object), new AssertFound(_handlerMock.Object), new AssertValidCast(_handlerMock.Object));
        }

        [Test]
        public void Positive_Convert_CreatesDefaultJob()
        {
            Dictionary<string, object> input = new() { { "JobID", JOB } };

            JobDTO actual = _converter.Convert(input);
            
            Assert.That(actual.JobID, Is.EqualTo(JOB));
        }

        [Test]
        public void Negative_KeyNotFound_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<NotFoundException>()))
                .Throws(new NotFoundException("JobID"));
            
            Assert.Throws<NotFoundException>(() => _converter.Convert(new Dictionary<string, object>()));
        }

        [Test]
        public void Negative_Convert_NullContent_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _converter.Convert(null!));
        }

        [Test]
        public void Negative_Convert_InvalidCast_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<InvalidCastException>()))
                .Throws<InvalidCastException>();
            
            Dictionary<string, object> input = new() { { "JobID", 12 } };
            
            Assert.Throws<InvalidCastException>(() => _converter.Convert(input));
        }
    }
}