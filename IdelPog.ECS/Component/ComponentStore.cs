namespace IdelPog.ECS.Component
{
    public readonly struct ComponentStore<T>() : IComponent where T : IComponent
    {
        private readonly List<T> _components = [];

        public void AddComponent(T component)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(T component)
        {
            throw new NotImplementedException();
        }

        public T[] GetAllComponents()
        {
            throw new NotImplementedException();
        }
    }
}