using IdelPog.Common.Factories;
using IdelPog.Flows.Builder;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;

namespace IdelPog.Flows.Types
{
    public readonly record struct FlowDescriptor
    {
        public required Type CommandType { get; init; }
        public required BufferMode ListeningMode { get; init; }
        public required IController Controller { get; init; }
        public required IDispatcher ErrorDispatcher { get; init; }
        public required IErrorFactory ErrorFactory { get; init; }
        public required string Description { get; init; }
    }
}