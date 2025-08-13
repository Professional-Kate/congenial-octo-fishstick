namespace IdelPog.ECS.Exceptions
{
    public class ComponentAlreadyExistsException : Exception
    {
        public ComponentAlreadyExistsException(object component) : base(string.Format(ExceptionMessages.COMPONENT_ALREADY_EXISTS, component))
        {
        }
    }
}