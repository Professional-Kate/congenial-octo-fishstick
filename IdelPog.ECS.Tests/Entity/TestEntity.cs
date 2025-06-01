using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Repository;

namespace IdelPog.ECS.Tests
{
    internal record TestEntity : Entity
    {
        public TestEntity(IRepository<Type, IComponent> components) : base(components)
        {
        }

        protected override void AddRequiredComponents()
        {
            AddComponent(new TestComponent());
        }
    }
}