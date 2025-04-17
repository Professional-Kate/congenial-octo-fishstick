using ContentHydrator.Converters;
using ContentHydrator.DTO;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
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
            _converter = new JobConverter(new AssertNotNull(_handlerMock.Object));
        }

        [Test]
        public void Positive_Convert_CreatesDefaultJob()
        {
            JobDTO actual = _converter.Convert(JOB);
            
            Assert.That(actual.JobID, Is.EqualTo(JOB));
        }

        [Test]
        public void Negative_Convert_NullContent_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _converter.Convert(null!));
        }
    }
}