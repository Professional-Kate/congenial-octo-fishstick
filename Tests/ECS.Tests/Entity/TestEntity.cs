using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests.Entity
{
    internal record TestEntity : ECS.Entity.Entity
    {
        public TestEntity(IDictionary<Type, IComponent> components)
            : base(components)
        {
        }
    }
}