using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Contracts;
using IdelPog.Core.Logging.Writer;
using Moq;

namespace IdelPog.Core.Tests.Logging
{
    [TestFixture]
    public class LoggingServiceTest
    {
        private ILogger _loggingService;
        private Mock<ILogWriter> _logWriterMock;

        private int[] _messages;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _messages = [1, 2, 3, 4, 5];
            
            _logWriterMock = new Mock<ILogWriter>();
            _loggingService = new LoggingService(_logWriterMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _logWriterMock.Reset();
        }
        
        private void VerifyWriterMock(int[] messages, LogLevel logLevel, LogDirection logDirection)
        {
            _logWriterMock.Verify(library => library.Write(logLevel, logDirection, messages), Times.Once);
        }

        [Test]
        public void Positive_Log_SingleMessage_LogsMessage()
        {
            const int message = 1;
            Assert.DoesNotThrow(() => _loggingService.Log(LogLevel.INFO, LogDirection.OUT, message));
            
            VerifyWriterMock([message],  LogLevel.INFO, LogDirection.OUT);
        }

        [Test]
        public void Positive_Log_MultipleMessages_LogsMessages()
        {
            Assert.DoesNotThrow(() => _loggingService.Log(LogLevel.INFO, LogDirection.IN, _messages));
            VerifyWriterMock(_messages, LogLevel.INFO, LogDirection.IN);
        }

        [Test]
        public void Positive_LogInfo_LogsAsInfo()
        {
            Assert.DoesNotThrow(() => _loggingService.LogInfo(LogDirection.IN, _messages));
            VerifyWriterMock(_messages, LogLevel.INFO, LogDirection.IN);
        }
        
        [Test]
        public void Positive_LogError_LogsAsError()
        {
            Assert.DoesNotThrow(() => _loggingService.LogError(LogDirection.OUT, _messages));
            VerifyWriterMock(_messages, LogLevel.ERROR, LogDirection.OUT);
        }
    }
}