namespace IdelPog.ECS.Assertion.Interface
{
    public interface IComponentAssertion
    {
        public void AssertUnique<TComponent>(bool exists);

        public void AssertFound<TComponent>(bool found);
    }
}