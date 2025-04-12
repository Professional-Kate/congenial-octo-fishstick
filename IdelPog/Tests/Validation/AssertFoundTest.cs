using IdelPogTemp.Main.Validation.Assertions;
using IdelPogTemp.Main.Validation.Assertions.Handlers;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;
using NUnit.Framework;

namespace IdelPogTemp.Tests.Validation
{
    [TestFixture]
    public class AssertFoundTest
    {
        private IAssertFound _assertFound { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertFound = new AssertFound(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertItemIsFound_PassedTrue()
        {
            Assert.DoesNotThrow(() => _assertFound.AssertItemIsFound(1, () => true));
        }

        [Test]
        public void Negative_AssertItemIsFound_PassedFalse_Throws()
        {
            Assert.Throws<NotFoundException>(() => _assertFound.AssertItemIsFound(1, () => false));
        }
    }
}