using IdelPog.ECS.Component;

namespace IdelPog.ECS.Exceptions
{
    public class ComponentAlreadyExistsException : Exception
    {
        public ComponentAlreadyExistsException(Object component) : base(string.Format(ExceptionMessages.COMPONENT_ALREADY_EXISTS, component)) { }
    }
}