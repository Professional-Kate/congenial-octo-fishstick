namespace IdelPog.ECS.Exceptions
{
    public class ComponentArrayEmptyException : Exception
    {
        public ComponentArrayEmptyException() : base(ExceptionMessages.COMPONENT_ARRAY_EMPTY) { }

    }
}