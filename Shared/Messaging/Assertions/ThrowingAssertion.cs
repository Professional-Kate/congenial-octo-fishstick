using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class ThrowingAssertion : BaseAssertion, IThrowingAssertion
    {
        public ThrowingAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertDoesNotThrow<TMessage>(TMessage message, ISingleController<TMessage> controller)
        {
            Assert<ControllerThrownException>(() =>
            {
                try
                {
                    controller.HandleMessage(message);
                }
                catch (Exception exception)
                {
                    ThrowException(controller.GetType().Name,  exception);
                }
            });
        }

        public void AssertDoesNotThrow<TMessage>(IReadOnlyList<TMessage> message, IBatchedController<TMessage> controller)
        {
            Assert<ControllerThrownException>(() =>
            {
                try
                {
                    controller.HandleMessages(message);
                }
                catch (Exception exception)
                {
                    ThrowException(controller.GetType().Name,  exception);
                }
            });
        }

        private static void ThrowException(string controllerName, Exception exception)
        {
            throw new ControllerThrownException(controllerName, exception);
        }
    }
}