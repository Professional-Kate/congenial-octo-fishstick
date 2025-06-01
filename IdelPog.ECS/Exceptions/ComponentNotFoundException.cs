using IdelPog.ECS.Component;

namespace IdelPog.ECS.Exceptions
{
    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(IComponent component) : base(string.Format(ExceptionMessages.COMPONENT_NOT_FOUND, component)) { }
    }
}