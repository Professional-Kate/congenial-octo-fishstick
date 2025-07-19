namespace IdelPog.ECS.Assertions
{
    public interface IAssertArrayNotNull
    {
        public void Handle<T>(T[]? arrayNotNull);
    }
}