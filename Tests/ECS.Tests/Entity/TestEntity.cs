using IdelPog.Core.Repository.Asset;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests.Entity
{
    internal record TestEntity : ECS.Entity.Entity
    {
        public TestEntity(IAssetRepository<Type, IComponent> components)
            : base(components)
        {
        }
    }
}