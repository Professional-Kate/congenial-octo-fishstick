using IdelPog.Core.Messaging.Listener;

namespace IdelPog.Core.Flows.Registry
{
    public interface IRegisterReader
    {
        public IReadOnlyList<IListener> GetListeners();
    }
}