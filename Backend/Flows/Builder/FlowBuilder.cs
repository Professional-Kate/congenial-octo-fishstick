using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using IdelPog.Common.Factories;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;

namespace IdelPog.Flows.Builder
{
    public class FlowBuilder
    {
        private Type? _commandType;
        private IController? _controller;
        private IDispatcher? _responseDispatcher;
        private IErrorFactory? _errorFactory;
        private string? _description;
        private BufferMode _bufferMode;

        public FlowBuilder ForCommand(Type commandType)
        {
            _commandType = commandType;
            return this;
        }
        
        public FlowBuilder WithController(IController controller)
        {
            _controller = controller;
            return this;
        }

        public FlowBuilder WithResponseDispatcher(IDispatcher responseDispatcher)
        {
            _responseDispatcher = responseDispatcher;
            return this;
        }

        public FlowBuilder WithErrorFactory(IErrorFactory errorFactory)
        {
            _errorFactory = errorFactory;
            return this;
        }
        
        public FlowBuilder SetDispatchMode(BufferMode bufferMode)
        {
            _bufferMode = bufferMode;
            return this;
        }

        public FlowBuilder SetDescription(Type commandName, Type responseName, Type errorName)
        {
            const string baseString = "Handles {0}. Success {1}. Failure {2}.";
            _description = string.Format(baseString, commandName.Name, responseName.Name, errorName.Name);
            return this;
        }
        
        public FlowDescriptor Build()
        {
            Validate();
            
            return new FlowDescriptor
            {
                CommandType = _commandType,
                ListeningMode = _bufferMode,
                Controller = _controller,
                ResponseDispatcher = _responseDispatcher,
                ErrorFactory = _errorFactory,
                Description = _description
            };
        }

        [MemberNotNull(nameof(_controller), nameof(_errorFactory), nameof(_description),  nameof(_responseDispatcher), nameof(_commandType))]
        private void Validate()
        {
            Debug.Assert(_commandType != null, nameof(_commandType) + " != null");
            Debug.Assert(_controller != null, nameof(_controller) + " != null");
            Debug.Assert(_errorFactory != null, nameof(_errorFactory) + " != null");
            Debug.Assert(_description != null, nameof(_description) + " != null");
            Debug.Assert(_responseDispatcher != null, nameof(_responseDispatcher) + " != null");
        }
    }
}