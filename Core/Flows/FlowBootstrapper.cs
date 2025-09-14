using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Flows
{
    public static class FlowBootstrapper
    {
        /// <summary>
        /// Creates a new <see cref="FlowRegister"/> 
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <returns>A ready to use <see cref="FlowRegister"/></returns>
        public static FlowRegister CreateFlowRegister(IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            ILogWriter writer = new ConsoleWriter();
            ILogger logger = new LoggingService(writer);
            
            FlowRegister flowRegister = new(bufferManager, logger, objectNullAssertion, collectionAssertion, uniqueAssertion);
            return flowRegister;
        }

        /// <summary>
        /// Gets each <see cref="IListener"/> from the <see cref="IRegisterReader"/> and subscribes them
        /// </summary>
        /// <param name="reader">Where each <see cref="IListener"/> should be registered</param>
        /// <param name="bufferMessenger">Used to subscribe each Listener</param>
        public static void SubscribeFlows(IRegisterReader reader, IBufferMessenger bufferMessenger)
        {
            foreach (IListener listener in reader.GetListeners())
            {
                bufferMessenger.Subscribe(listener);
            }
        }
    }
}