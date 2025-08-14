using IdelPog.Console;
using IdelPog.Console.Exceptions;
using IdelPog.Console.Runtime.Input;
using IdelPog.Console.Runtime.Input.Exceptions;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Integration.Tests.Console.Permission;

namespace IdelPog.Integration.Tests.Console
{
    [TestFixture]
    public class ScheduleDomainFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;
        private ScheduleControlListener _scheduleControlListener;

        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
            TestPermissionService.SendAddPermissionCall(_inputHandler, Domain.SCHEDULE);

            _scheduleControlListener = new ScheduleControlListener();
            ManagedSubscribe(_scheduleControlListener);
        }

        private static IEnumerable<TestCaseData> ValidScheduleCases()
        {
            yield return new TestCaseData(
                new[] { "SCHEDULE", "start" },
                new ScheduleControl { ControlAction = ControlAction.START }
            ).SetName("Start_Schedule");

            yield return new TestCaseData(
                new[] { "schedule", "STOP" },
                new ScheduleControl { ControlAction = ControlAction.STOP }
            ).SetName("Stop_Schedule");
        }

        [TestCaseSource(nameof(ValidScheduleCases))]
        public void Positive_UpdateSchedule_SendsScheduleControlCommand(string[] arguments, ScheduleControl expectedCommand)
        {
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            Assert.Multiple(() =>
            {
                Assert.That(_scheduleControlListener.WasCalled, Is.True);
                Assert.That(_scheduleControlListener.ScheduleControl.ControlAction, Is.EqualTo(expectedCommand.ControlAction));
            });
        }

        private static IEnumerable<TestCaseData> InvalidScheduleCases()
        {
            yield return new TestCaseData(
                new[] { "UNKNOWN", "start" },
                typeof(FailedEnumParseException)
            ).SetName("UnknownDomain_ThrowsFailedEnumParse");

            yield return new TestCaseData(
                new[] { "schedule", "UPDATE" },
                typeof(FailedEnumParseException)
            ).SetName("UnknownAction_ThrowsFailedEnumParse");

            yield return new TestCaseData(
                new[] { "start", "Schedule" },
                typeof(FailedEnumParseException)
            ).SetName("WrongArgumentOrder_ThrowsFailedEnumParse");

            yield return new TestCaseData(
                new[] { "schedule", "start", "0" },
                typeof(InvalidArgumentCountException)
            ).SetName("TooManyArguments_ThrowsInvalidArgumentCount");

            yield return new TestCaseData(
                new[] { "schedule" },
                typeof(EmptySpanException)
            ).SetName("MissingArguments_ThrowsEmptySpan");

            yield return new TestCaseData(
                new string[] { },
                typeof(EmptySpanException)
            ).SetName("EmptyArguments_ThrowsEmptySpan");
        }

        [TestCaseSource(nameof(InvalidScheduleCases))]
        public void Positive_UpdateSchedule_BadArguments_NoSentCommand_Throws(string[] arguments, Type exception)
        {
            Assert.Throws(exception, () => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
            Assert.That(_scheduleControlListener.WasCalled, Is.False);
        }

        [Test]
        public void Negative_PermissionDenied_NoUpdate_Throws()
        {
            TestPermissionService.SendRemovePermissionCall(_inputHandler, Domain.SCHEDULE);
            Assert.Throws<DomainPermissionDeniedException>(() => _inputHandler.Input(new ReadOnlySpan<string>(["schedule", "start"])));
            Assert.That(_scheduleControlListener.WasCalled, Is.False);
        }
    }
}