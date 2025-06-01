namespace IdelPog.ECS.Component
{
    public readonly struct ImmutableComponentStore<T>(T[] components) : IComponent where T : ICloneableComponent<T>
    {
        public T[] GetAllComponents()
        {
            throw new NotImplementedException();
        }
    }
}