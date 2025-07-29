using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace IdelPog.Flows.Builder
{
    public class FlowBuilder<TCommand>
    {
        public static FlowBuilder<TCommand> ForCommand => new();
        
        private Type? _controllerType;
        private Type? _resultType;
        private Type? _errorType;
        private Type? _mediatorType;
        
        public FlowBuilder<TCommand> WithController<TController>()
        {
            _controllerType = typeof(TController);
            return this;
        }

        public FlowBuilder<TCommand> WithMediator<TMediator>()
        {
            _mediatorType = typeof(TMediator);
            return this;
        }

        public FlowBuilder<TCommand> OnSuccess<TResult>()
        {
            _resultType = typeof(TResult);
            return this;
        }

        public FlowBuilder<TCommand> OnError<TError>()
        {
            _errorType = typeof(TError);
            return this;
        }

        public FlowDescriptor Build()
        {
            Validate();
            
            return new FlowDescriptor
            {
                CommandType = typeof(TCommand),
                ControllerType = _controllerType,
                MediatorType = _mediatorType,
                SuccessResultType = _resultType,
                ErrorResultType = _errorType,
            };
        }

        [MemberNotNull(nameof(_controllerType), nameof(_resultType), nameof(_errorType), nameof(_mediatorType))]
        private void Validate()
        {
            Debug.Assert(_controllerType != null, nameof(_controllerType) + " != null");
            Debug.Assert(_resultType != null, nameof(_resultType) + " != null");
            Debug.Assert(_errorType != null, nameof(_errorType) + " != null");
            Debug.Assert(_mediatorType != null, nameof(_mediatorType) + " != null");

        }
    }
}