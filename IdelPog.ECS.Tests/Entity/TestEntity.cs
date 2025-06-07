using IdelPog.ECS.Collection;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Tests
{
    internal record TestEntity : Entity
    {
        public TestEntity(IComponentMap components, IHandler handler) 
            : base(components, handler)
        {
        }
    }
}