using IdelPog.ECS.Component;

namespace IdelPog.ECS.Factory
{
    public interface IComponentStoreFactory
    {
        public ComponentStore<T> CreateComponentStore<T>(T[] components) where T : IComponent<T>;
    }
}