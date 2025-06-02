using IdelPog.ECS.Component;
using IdelPog.Infrastructure.Repository;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Tests
{
    internal record TestEntity : Entity
    {
        public TestEntity(IRepository<Type, IComponent> components, IHandler handler) 
            : base(components, handler)
        {
        }

        protected override void AddRequiredComponents()
        {
            AddComponent(new TestComponent());
        }
    }
}