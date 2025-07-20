namespace IdelPog.ECS.Assertions
{
    public interface IAssertComponentDoesNotExist
    {
        public void Handle(bool componentAlreadyExists, object componentContext);
    }
}