using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Factory
{
    public class ComponentStoreFactory : IComponentStoreFactory
    {
        public ComponentStore<T> CreateComponentStore<T>(T[] components) where T : IComponent<T>
        {
            return new ComponentStore<T>(components, new ThrowHandler());
        }
    }
}