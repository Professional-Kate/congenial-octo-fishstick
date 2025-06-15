using IdelPog.Common.Repository;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ECS.Tests
{
    internal record TestEntity : Entity
    {
        public TestEntity(IAssetRepository<Type, IComponent> components, IHandler handler) 
            : base(components, handler)
        {
        }
    }
}