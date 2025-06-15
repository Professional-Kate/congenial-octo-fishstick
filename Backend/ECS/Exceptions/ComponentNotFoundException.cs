namespace IdelPog.ECS.Exceptions
{
    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(Type componentType) : base(string.Format(ExceptionMessages.COMPONENT_NOT_FOUND, componentType)) { }
    }
}