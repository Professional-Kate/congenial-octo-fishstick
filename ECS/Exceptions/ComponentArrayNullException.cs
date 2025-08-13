namespace IdelPog.ECS.Exceptions
{
    public class ComponentArrayNullException : Exception
    {
        public ComponentArrayNullException() : base(ExceptionMessages.COMPONENT_ARRAY_NULL)
        {
        }
    }
}