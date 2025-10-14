using IdelPog.Core.Repository.Asset;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests
{
    internal record TestEntity : Entity.Entity
    {
        public TestEntity(IAssetRepository<Type, IComponent> components)
            : base(components)
        {
        }
    }
}