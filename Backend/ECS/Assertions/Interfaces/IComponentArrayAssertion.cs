namespace IdelPog.ECS.Assertions
{
    public interface IComponentArrayAssertion
    {
        public void AssertNotNull<T>(T[]? array);
        
        public void AssertNotEmpty<T>(T[] array);
        
        public void AssertHasElements<T>(T[]? array);
    }
}