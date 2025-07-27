namespace IdelPog.ECS.Assertions
{
    public interface IComponentAssertion
    {
        public void AssertUnique<TComponent>(bool exists);

        public void AssertFound<TComponent>(bool found);
    }
}