using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace IdelPog.Flows.Builder
{
    public class FlowBuilder
    {
        private Type? _commandType;
        private Type? _controllerType;
        private Type? _resultType;
        private Type? _errorType;
        private Type? _mediatorType;
        private DispatchMode _dispatchMode;
        
        public FlowBuilder ForCommand<TCommand>(DispatchMode dispatchMode)
        {
            _dispatchMode = dispatchMode;
            _commandType =  typeof(TCommand);
            return this;
        }
        
        public FlowBuilder WithController<TController>()
        {
            _controllerType = typeof(TController);
            return this;
        }

        public FlowBuilder WithMediator<TMediator>()
        {
            _mediatorType = typeof(TMediator);
            return this;
        }

        public FlowBuilder OnSuccess<TResult>()
        {
            _resultType = typeof(TResult);
            return this;
        }

        public FlowBuilder OnError<TError>()
        {
            _errorType = typeof(TError);
            return this;
        }

        public FlowDescriptor Build()
        {
            Validate();
            
            return new FlowDescriptor
            {
                CommandType = _commandType,
                DispatchMode = _dispatchMode,
                ControllerType = _controllerType,
                MediatorType = _mediatorType,
                SuccessResultType = _resultType,
                ErrorResultType = _errorType,
            };
        }

        [MemberNotNull(nameof(_commandType), nameof(_controllerType), nameof(_resultType), nameof(_errorType), nameof(_mediatorType))]
        private void Validate()
        {
            Debug.Assert(_commandType != null, nameof(_commandType) + " != null");
            Debug.Assert(_controllerType != null, nameof(_controllerType) + " != null");
            Debug.Assert(_resultType != null, nameof(_resultType) + " != null");
            Debug.Assert(_errorType != null, nameof(_errorType) + " != null");
            Debug.Assert(_mediatorType != null, nameof(_mediatorType) + " != null");

        }
    }
}