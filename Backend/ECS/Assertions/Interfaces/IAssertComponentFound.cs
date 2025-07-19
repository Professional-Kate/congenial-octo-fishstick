namespace IdelPog.ECS.Assertions
{
    public interface IAssertComponentFound
    {
        public void Handle(bool componentWasFound, Type componentContext);
    }
}