using IdelPog.Core.Flows.Registry;
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
        public static FlowRegister CreateFlowRegister(IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            
            FlowRegister flowRegister = new(bufferManager, objectNullAssertion, collectionAssertion, uniqueAssertion);
            return flowRegister;
        }

        public static void SubscribeFlows(IRegisterReader reader, IBufferMessenger bufferMessenger)
        {
            foreach (IListener listener in reader.GetListeners())
            {
                bufferMessenger.Subscribe(listener);
            }
        }
    }
}